using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class ExamTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();

        var exam = Exam.Create(id, "Math 101", "Algebra basics");

        Assert.Equal(id, exam.Id);
        Assert.Equal("Math 101", exam.Title);
        Assert.Equal("Algebra basics", exam.Description);
    }

    [Fact]
    public void Create_AllowsNullDescription()
    {
        var exam = Exam.Create(Guid.NewGuid(), "Untitled", null);

        Assert.Null(exam.Description);
    }
}
