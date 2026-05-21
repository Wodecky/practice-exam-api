using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PracticeExam.Infrastructure.Persistence;

namespace PracticeExam.Infrastructure.UnitTests.Persistence;

/// <summary>
/// Exercises <see cref="ExamRepository"/> and the <see cref="PracticeExamDbContext"/>
/// mapping against a real (in-memory) SQLite database.
/// </summary>
public sealed class ExamRepositoryTests : IDisposable
{
    // The sqitch schema marks created_at/updated_at NOT NULL; the API no longer
    // maps them, so inserts just supply a constant to satisfy the constraint.
    private const string Timestamp = "2026-01-01 10:00:00";

    private readonly SqliteConnection _connection;
    private readonly PracticeExamDbContext _dbContext;

    public ExamRepositoryTests()
    {
        // An in-memory SQLite database lives only as long as its connection is open.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<PracticeExamDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new PracticeExamDbContext(options);

        CreateSchema();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenThereAreNoExams()
    {
        var repository = new ExamRepository(_dbContext);

        var exams = await repository.GetAllAsync();

        Assert.Empty(exams);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsExamsOrderedById()
    {
        InsertExam("Second", id: "22222222222222222222222222222222");
        InsertExam("First", id: "11111111111111111111111111111111");
        var repository = new ExamRepository(_dbContext);

        var exams = await repository.GetAllAsync();

        Assert.Collection(
            exams,
            exam => Assert.Equal("First", exam.Title),
            exam => Assert.Equal("Second", exam.Title));
    }

    [Fact]
    public async Task GetAllAsync_MapsHexIdToGuidAndPreservesFields()
    {
        const string hexId = "a1b2c3d4e5f6470890123456789abcde";
        InsertExam("Biology", id: hexId, description: null);
        var repository = new ExamRepository(_dbContext);

        var exam = Assert.Single(await repository.GetAllAsync());

        Assert.Equal(Guid.Parse(hexId), exam.Id);
        Assert.Equal("Biology", exam.Title);
        Assert.Null(exam.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenExamDoesNotExist()
    {
        var repository = new ExamRepository(_dbContext);

        var exam = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(exam);
    }

    [Fact]
    public async Task GetByIdAsync_LoadsExamWithItsQuestionsAndAnswers()
    {
        const string examId = "a1b2c3d4e5f6470890123456789abcde";
        const string questionId = "b1b2c3d4e5f6470890123456789abcde";
        InsertExam("Geography", id: examId);
        InsertQuestion(questionId, examId, "Capital of France?");
        InsertAnswer(questionId, "Paris", isCorrect: true);
        InsertAnswer(questionId, "London", isCorrect: false);
        var repository = new ExamRepository(_dbContext);

        var exam = await repository.GetByIdAsync(Guid.Parse(examId));

        Assert.NotNull(exam);
        Assert.Equal(Guid.Parse(examId), exam.Id);
        var question = Assert.Single(exam.Questions);
        Assert.Equal(Guid.Parse(questionId), question.Id);
        Assert.Equal("Capital of France?", question.Text);
        Assert.Equal(2, question.Answers.Count);
        Assert.Contains(question.Answers, answer => answer is { Text: "Paris", IsCorrect: true });
        Assert.Contains(question.Answers, answer => answer is { Text: "London", IsCorrect: false });
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsExamWithNoQuestions_WhenItHasNone()
    {
        const string examId = "c1b2c3d4e5f6470890123456789abcde";
        InsertExam("Empty", id: examId);
        var repository = new ExamRepository(_dbContext);

        var exam = await repository.GetByIdAsync(Guid.Parse(examId));

        Assert.NotNull(exam);
        Assert.Empty(exam.Questions);
    }

    private void CreateSchema()
    {
        // A minimal read-side mirror of the sqitch-managed schema. Triggers, indexes,
        // the is_correct CHECK constraint, and ON DELETE CASCADE are intentionally
        // omitted — these read-only tests don't exercise them.
        using var command = _connection.CreateCommand();
        command.CommandText =
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
        command.ExecuteNonQuery();
    }

    private void InsertExam(
        string title,
        string? id = null,
        string? description = "sample description")
    {
        using var command = _connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO exams (id, title, description, created_at, updated_at)
            VALUES ($id, $title, $description, $timestamp, $timestamp);
            """;
        command.Parameters.AddWithValue("$id", id ?? Guid.NewGuid().ToString("N"));
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$description", (object?)description ?? DBNull.Value);
        command.Parameters.AddWithValue("$timestamp", Timestamp);
        command.ExecuteNonQuery();
    }

    private void InsertQuestion(string id, string examId, string text)
    {
        using var command = _connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO questions (id, exam_id, text, created_at, updated_at)
            VALUES ($id, $examId, $text, $timestamp, $timestamp);
            """;
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$examId", examId);
        command.Parameters.AddWithValue("$text", text);
        command.Parameters.AddWithValue("$timestamp", Timestamp);
        command.ExecuteNonQuery();
    }

    private void InsertAnswer(string questionId, string text, bool isCorrect)
    {
        using var command = _connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO answers (id, question_id, text, is_correct, created_at, updated_at)
            VALUES ($id, $questionId, $text, $isCorrect, $timestamp, $timestamp);
            """;
        command.Parameters.AddWithValue("$id", Guid.NewGuid().ToString("N"));
        command.Parameters.AddWithValue("$questionId", questionId);
        command.Parameters.AddWithValue("$text", text);
        command.Parameters.AddWithValue("$isCorrect", isCorrect ? 1 : 0);
        command.Parameters.AddWithValue("$timestamp", Timestamp);
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
