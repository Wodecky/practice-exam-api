using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class QuestionTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();
        var examId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Utc);

        var question = Question.Create(id, examId, "What is 2 + 2?", createdAt, updatedAt);

        Assert.Equal(id, question.Id);
        Assert.Equal(examId, question.ExamId);
        Assert.Equal("What is 2 + 2?", question.Text);
        Assert.Equal(createdAt, question.CreatedAt);
        Assert.Equal(updatedAt, question.UpdatedAt);
    }

    [Fact]
    public void Create_HasNoAnswers_WhenNoneSupplied()
    {
        var question = Question.Create(
            Guid.NewGuid(), Guid.NewGuid(), "Q", DateTime.UtcNow, DateTime.UtcNow);

        Assert.Empty(question.Answers);
    }

    [Fact]
    public void Create_AttachesSuppliedAnswers()
    {
        var answer = Answer.Create(
            Guid.NewGuid(), Guid.NewGuid(), "42", true, DateTime.UtcNow, DateTime.UtcNow);

        var question = Question.Create(
            Guid.NewGuid(), Guid.NewGuid(), "Q", DateTime.UtcNow, DateTime.UtcNow, [answer]);

        Assert.Same(answer, Assert.Single(question.Answers));
    }
}
