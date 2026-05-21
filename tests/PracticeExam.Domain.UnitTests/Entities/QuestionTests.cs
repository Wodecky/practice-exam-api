using PracticeExam.Domain.Entities;

namespace PracticeExam.Domain.UnitTests.Entities;

public class QuestionTests
{
    [Fact]
    public void Create_SetsEveryProperty()
    {
        var id = Guid.NewGuid();
        var examId = Guid.NewGuid();

        var question = Question.Create(id, examId, "What is 2 + 2?");

        Assert.Equal(id, question.Id);
        Assert.Equal(examId, question.ExamId);
        Assert.Equal("What is 2 + 2?", question.Text);
    }

    [Fact]
    public void Create_HasNoAnswers_WhenNoneSupplied()
    {
        var question = Question.Create(Guid.NewGuid(), Guid.NewGuid(), "Q");

        Assert.Empty(question.Answers);
    }

    [Fact]
    public void Create_AttachesSuppliedAnswers()
    {
        var answer = Answer.Create(Guid.NewGuid(), Guid.NewGuid(), "42", true);

        var question = Question.Create(Guid.NewGuid(), Guid.NewGuid(), "Q", [answer]);

        Assert.Same(answer, Assert.Single(question.Answers));
    }
}
