using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PracticeExam.Infrastructure.Persistence;

namespace PracticeExam.Api.IntegrationTests;

/// <summary>
/// Boots the real API over a throwaway SQLite file seeded with one exam that has
/// a single question and two answers. The production
/// <see cref="PracticeExamDbContext"/> registration is replaced so tests never
/// touch the developer's real database.
/// </summary>
public sealed class ExamsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"practice_exam_it_{Guid.NewGuid():N}.db");

    public Guid SeededExamId { get; } = Guid.NewGuid();

    public Guid SeededQuestionId { get; } = Guid.NewGuid();

    public ExamsApiFactory() => SeedDatabase();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<PracticeExamDbContext>>();
            services.AddDbContext<PracticeExamDbContext>(options =>
                options.UseSqlite($"Data Source={_databasePath}"));
        });
    }

    private void SeedDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using (var create = connection.CreateCommand())
        {
            // A minimal read-side mirror of the sqitch-managed schema. Triggers, indexes,
            // the is_correct CHECK constraint, and ON DELETE CASCADE are intentionally
            // omitted — these read-only tests don't exercise them.
            create.CommandText =
                """
                CREATE TABLE exams (
                    id          TEXT PRIMARY KEY,
                    title       TEXT NOT NULL,
                    description TEXT,
                    created_at  TEXT NOT NULL,
                    updated_at  TEXT NOT NULL
                );
                CREATE TABLE questions (
                    id         TEXT PRIMARY KEY,
                    exam_id    TEXT NOT NULL REFERENCES exams(id),
                    text       TEXT NOT NULL,
                    created_at TEXT NOT NULL,
                    updated_at TEXT NOT NULL
                );
                CREATE TABLE answers (
                    id          TEXT PRIMARY KEY,
                    question_id TEXT NOT NULL REFERENCES questions(id),
                    text        TEXT NOT NULL,
                    is_correct  INTEGER NOT NULL DEFAULT 0,
                    created_at  TEXT NOT NULL,
                    updated_at  TEXT NOT NULL
                );
                """;
            create.ExecuteNonQuery();
        }

        using (var insertExam = connection.CreateCommand())
        {
            insertExam.CommandText =
                """
                INSERT INTO exams (id, title, description, created_at, updated_at)
                VALUES ($id, 'Integration Exam', 'Seeded for integration tests',
                        '2026-01-01 10:00:00', '2026-01-01 10:00:00');
                """;
            insertExam.Parameters.AddWithValue("$id", SeededExamId.ToString("N"));
            insertExam.ExecuteNonQuery();
        }

        using (var insertQuestion = connection.CreateCommand())
        {
            insertQuestion.CommandText =
                """
                INSERT INTO questions (id, exam_id, text, created_at, updated_at)
                VALUES ($id, $examId, 'What is 2 + 2?',
                        '2026-01-01 10:00:00', '2026-01-01 10:00:00');
                """;
            insertQuestion.Parameters.AddWithValue("$id", SeededQuestionId.ToString("N"));
            insertQuestion.Parameters.AddWithValue("$examId", SeededExamId.ToString("N"));
            insertQuestion.ExecuteNonQuery();
        }

        InsertAnswer(connection, "4", isCorrect: true, "2026-01-01 10:00:00");
        InsertAnswer(connection, "5", isCorrect: false, "2026-01-01 10:00:01");
    }

    private void InsertAnswer(
        SqliteConnection connection,
        string text,
        bool isCorrect,
        string createdAt)
    {
        using var insert = connection.CreateCommand();
        insert.CommandText =
            """
            INSERT INTO answers (id, question_id, text, is_correct, created_at, updated_at)
            VALUES ($id, $questionId, $text, $isCorrect, $createdAt, $createdAt);
            """;
        insert.Parameters.AddWithValue("$id", Guid.NewGuid().ToString("N"));
        insert.Parameters.AddWithValue("$questionId", SeededQuestionId.ToString("N"));
        insert.Parameters.AddWithValue("$text", text);
        insert.Parameters.AddWithValue("$isCorrect", isCorrect ? 1 : 0);
        insert.Parameters.AddWithValue("$createdAt", createdAt);
        insert.ExecuteNonQuery();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Release SQLite's pooled file handle before deleting the temp file.
            SqliteConnection.ClearAllPools();
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
    }
}
