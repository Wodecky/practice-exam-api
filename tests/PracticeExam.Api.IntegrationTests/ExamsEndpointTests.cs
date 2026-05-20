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
}
