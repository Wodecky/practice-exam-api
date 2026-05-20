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
    public async Task GetAllAsync_ReturnsExamsOrderedByCreatedAt()
    {
        InsertExam("Newer", "2026-02-01 10:00:00");
        InsertExam("Older", "2026-01-01 10:00:00");
        var repository = new ExamRepository(_dbContext);

        var exams = await repository.GetAllAsync();

        Assert.Collection(
            exams,
            exam => Assert.Equal("Older", exam.Title),
            exam => Assert.Equal("Newer", exam.Title));
    }

    [Fact]
    public async Task GetAllAsync_MapsHexIdToGuidAndPreservesFields()
    {
        const string hexId = "a1b2c3d4e5f6470890123456789abcde";
        InsertExam("Biology", "2026-01-01 10:00:00", id: hexId, description: null);
        var repository = new ExamRepository(_dbContext);

        var exam = Assert.Single(await repository.GetAllAsync());

        Assert.Equal(Guid.Parse(hexId), exam.Id);
        Assert.Equal("Biology", exam.Title);
        Assert.Null(exam.Description);
        Assert.Equal(new DateTime(2026, 1, 1, 10, 0, 0), exam.CreatedAt);
    }

    private void CreateSchema()
    {
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
            """;
        command.ExecuteNonQuery();
    }

    private void InsertExam(
        string title,
        string createdAt,
        string? id = null,
        string? description = "sample description")
    {
        using var command = _connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO exams (id, title, description, created_at, updated_at)
            VALUES ($id, $title, $description, $createdAt, $createdAt);
            """;
        command.Parameters.AddWithValue("$id", id ?? Guid.NewGuid().ToString("N"));
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$description", (object?)description ?? DBNull.Value);
        command.Parameters.AddWithValue("$createdAt", createdAt);
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
