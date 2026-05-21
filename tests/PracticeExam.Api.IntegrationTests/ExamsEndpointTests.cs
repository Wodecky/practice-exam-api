using System.Net;
using System.Net.Http.Json;
using PracticeExam.Application.Exams;

namespace PracticeExam.Api.IntegrationTests;

public sealed class ExamsEndpointTests(ExamsApiFactory factory) : IClassFixture<ExamsApiFactory>
{
    [Fact]
    public async Task GetExams_ReturnsOkWithTheSeededExam()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/exams");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exams = await response.Content.ReadFromJsonAsync<List<ExamResponse>>();

        Assert.NotNull(exams);
        var exam = Assert.Single(exams);
        Assert.Equal(factory.SeededExamId, exam.Id);
        Assert.Equal("Integration Exam", exam.Title);
        Assert.Equal("Seeded for integration tests", exam.Description);
    }

    [Fact]
    public async Task GetExamById_ReturnsOkWithQuestionsAndAnswers()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/exams/{factory.SeededExamId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exam = await response.Content.ReadFromJsonAsync<ExamDetailResponse>();

        Assert.NotNull(exam);
        Assert.Equal(factory.SeededExamId, exam.Id);
        Assert.Equal("Integration Exam", exam.Title);

        var question = Assert.Single(exam.Questions);
        Assert.Equal(factory.SeededQuestionId, question.Id);
        Assert.Equal("What is 2 + 2?", question.Text);

        Assert.Collection(
            question.Answers,
            answer =>
            {
                Assert.Equal("4", answer.Text);
                Assert.True(answer.IsCorrect);
            },
            answer =>
            {
                Assert.Equal("5", answer.Text);
                Assert.False(answer.IsCorrect);
            });
    }

    [Fact]
    public async Task GetExamById_ReturnsNotFound_WhenExamDoesNotExist()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/exams/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
