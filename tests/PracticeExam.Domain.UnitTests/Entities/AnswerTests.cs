using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class AnswerTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Utc);

        var answer = Answer.Create(id, questionId, "Paris", true, createdAt, updatedAt);

        Assert.Equal(id, answer.Id);
        Assert.Equal(questionId, answer.QuestionId);
        Assert.Equal("Paris", answer.Text);
        Assert.True(answer.IsCorrect);
        Assert.Equal(createdAt, answer.CreatedAt);
        Assert.Equal(updatedAt, answer.UpdatedAt);
    }

    [Fact]
    public void Create_AllowsIncorrectAnswer()
    {
        var answer = Answer.Create(
            Guid.NewGuid(), Guid.NewGuid(), "London", false, DateTime.UtcNow, DateTime.UtcNow);

        Assert.False(answer.IsCorrect);
    }
}
