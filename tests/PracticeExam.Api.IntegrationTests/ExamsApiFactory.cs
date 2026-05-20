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
/// Boots the real API over a throwaway SQLite file seeded with one exam.
/// The production <see cref="PracticeExamDbContext"/> registration is replaced
/// so tests never touch the developer's real database.
/// </summary>
public sealed class ExamsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"practice_exam_it_{Guid.NewGuid():N}.db");

    public Guid SeededExamId { get; } = Guid.NewGuid();

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
            create.CommandText =
                """
                CREATE TABLE exams (
                    id          TEXT PRIMARY KEY,
                    title       TEXT NOT NULL,
                    description TEXT,
                    created_at  TEXT NOT NULL,
                    updated_at  TEXT NOT NULL
                );
                """;
            create.ExecuteNonQuery();
        }

        using var insert = connection.CreateCommand();
        insert.CommandText =
            """
            INSERT INTO exams (id, title, description, created_at, updated_at)
            VALUES ($id, 'Integration Exam', 'Seeded for integration tests',
                    '2026-01-01 10:00:00', '2026-01-01 10:00:00');
            """;
        insert.Parameters.AddWithValue("$id", SeededExamId.ToString("N"));
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
