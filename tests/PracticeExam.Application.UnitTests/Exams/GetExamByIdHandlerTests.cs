using PracticeExam.Application.Exams;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Application.UnitTests.Exams;

public class GetExamByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsNull_WhenExamDoesNotExist()
    {
        var handler = new GetExamByIdHandler(new FakeExamRepository());

        var result = await handler.HandleAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_MapsExamWithQuestionsAndAnswers()
    {
        var examId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var correct = Answer.Create(Guid.NewGuid(), questionId, "4", true);
        var wrong = Answer.Create(Guid.NewGuid(), questionId, "5", false);
        var question = Question.Create(questionId, examId, "2 + 2?", [correct, wrong]);
        var exam = Exam.Create(examId, "Math", "Basics", [question]);
        var handler = new GetExamByIdHandler(new FakeExamRepository(exam));

        var result = await handler.HandleAsync(examId);

        Assert.NotNull(result);
        Assert.Equal(examId, result.Id);
        Assert.Equal("Math", result.Title);
        var mappedQuestion = Assert.Single(result.Questions);
        Assert.Equal(questionId, mappedQuestion.Id);
        Assert.Equal("2 + 2?", mappedQuestion.Text);
        Assert.Collection(
            mappedQuestion.Answers,
            answer => Assert.True(answer.IsCorrect),
            answer => Assert.False(answer.IsCorrect));
    }

    [Fact]
    public async Task HandleAsync_OrdersQuestionsAndAnswersById()
    {
        var examId = Guid.NewGuid();
        var firstId = new Guid("00000000-0000-0000-0000-000000000001");
        var secondId = new Guid("00000000-0000-0000-0000-000000000002");
        var laterAnswer = Answer.Create(secondId, firstId, "later", false);
        var earlierAnswer = Answer.Create(firstId, firstId, "earlier", true);
        var laterQuestion = Question.Create(secondId, examId, "later");
        var earlierQuestion = Question.Create(
            firstId, examId, "earlier", [laterAnswer, earlierAnswer]);
        var exam = Exam.Create(examId, "Exam", null, [laterQuestion, earlierQuestion]);
        var handler = new GetExamByIdHandler(new FakeExamRepository(exam));

        var result = await handler.HandleAsync(examId);

        Assert.NotNull(result);
        Assert.Collection(
            result.Questions,
            question =>
            {
                Assert.Equal("earlier", question.Text);
                Assert.Collection(
                    question.Answers,
                    answer => Assert.Equal("earlier", answer.Text),
                    answer => Assert.Equal("later", answer.Text));
            },
            question => Assert.Equal("later", question.Text));
    }
}
