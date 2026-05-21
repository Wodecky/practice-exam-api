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
        var createdAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var correct = Answer.Create(Guid.NewGuid(), questionId, "4", true, createdAt, createdAt);
        var wrong = Answer.Create(Guid.NewGuid(), questionId, "5", false, createdAt, createdAt);
        var question = Question.Create(
            questionId, examId, "2 + 2?", createdAt, createdAt, [correct, wrong]);
        var exam = Exam.Create(examId, "Math", "Basics", createdAt, createdAt, [question]);
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
    public async Task HandleAsync_OrdersQuestionsAndAnswersByCreatedAt()
    {
        var examId = Guid.NewGuid();
        var baseTime = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var qId = Guid.NewGuid();
        var laterAnswer = Answer.Create(
            Guid.NewGuid(), qId, "later", false, baseTime.AddMinutes(2), baseTime);
        var earlierAnswer = Answer.Create(
            Guid.NewGuid(), qId, "earlier", true, baseTime.AddMinutes(1), baseTime);
        var laterQuestion = Question.Create(
            Guid.NewGuid(), examId, "later", baseTime.AddHours(2), baseTime);
        var earlierQuestion = Question.Create(
            qId, examId, "earlier", baseTime.AddHours(1), baseTime, [laterAnswer, earlierAnswer]);
        var exam = Exam.Create(
            examId, "Exam", null, baseTime, baseTime, [laterQuestion, earlierQuestion]);
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
