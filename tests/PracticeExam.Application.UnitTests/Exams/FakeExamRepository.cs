using PracticeExam.Application.Common.Interfaces;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Application.UnitTests.Exams;

/// <summary>
/// In-memory <see cref="IExamRepository"/> stub that returns a fixed set of exams.
/// </summary>
internal sealed class FakeExamRepository(params Exam[] exams) : IExamRepository
{
    public Task<IReadOnlyList<Exam>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Exam>>(exams);
    }
}
