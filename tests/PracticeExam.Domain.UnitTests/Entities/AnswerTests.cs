using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class AnswerTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var answer = Answer.Create(id, questionId, "Paris", true);

        Assert.Equal(id, answer.Id);
        Assert.Equal(questionId, answer.QuestionId);
        Assert.Equal("Paris", answer.Text);
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void Create_AllowsIncorrectAnswer()
    {
        var answer = Answer.Create(Guid.NewGuid(), Guid.NewGuid(), "London", false);

        Assert.False(answer.IsCorrect);
    }
}
