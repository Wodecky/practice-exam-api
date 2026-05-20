using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class ExamTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Utc);

        var exam = Exam.Create(id, "Math 101", "Algebra basics", createdAt, updatedAt);

        Assert.Equal(id, exam.Id);
        Assert.Equal("Math 101", exam.Title);
        Assert.Equal("Algebra basics", exam.Description);
        Assert.Equal(createdAt, exam.CreatedAt);
        Assert.Equal(updatedAt, exam.UpdatedAt);
    }

    [Fact]
    public void Create_AllowsNullDescription()
    {
        var exam = Exam.Create(Guid.NewGuid(), "Untitled", null, DateTime.UtcNow, DateTime.UtcNow);

        Assert.Null(exam.Description);
    }
}
