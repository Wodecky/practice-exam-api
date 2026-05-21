using PracticeExam.Application.Exams;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Application.UnitTests.Exams;

public class GetExamsHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsEveryFieldOntoTheResponse()
    {
        var id = Guid.NewGuid();
        var exam = Exam.Create(id, "History", "World War II");
        var handler = new GetExamsHandler(new FakeExamRepository(exam));

        var result = await handler.HandleAsync();

        var response = Assert.Single(result);
        Assert.Equal(id, response.Id);
        Assert.Equal("History", response.Title);
        Assert.Equal("World War II", response.Description);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyList_WhenRepositoryHasNoExams()
    {
        var handler = new GetExamsHandler(new FakeExamRepository());

        var result = await handler.HandleAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_PreservesRepositoryOrder()
    {
        var first = Exam.Create(Guid.NewGuid(), "First", null);
        var second = Exam.Create(Guid.NewGuid(), "Second", null);
        var handler = new GetExamsHandler(new FakeExamRepository(first, second));

        var result = await handler.HandleAsync();

        Assert.Collection(
            result,
            exam => Assert.Equal("First", exam.Title),
            exam => Assert.Equal("Second", exam.Title));
    }
}
