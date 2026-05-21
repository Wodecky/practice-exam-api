using PracticeExam.Application.Common.Interfaces;

namespace PracticeExam.Application.Exams;

/// <summary>
/// Use case: list every exam.
/// </summary>
public sealed class GetExamsHandler(IExamRepository examRepository)
{
    public async Task<IReadOnlyList<ExamResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var exams = await examRepository.GetAllAsync(cancellationToken);

        return exams
            .Select(exam => new ExamResponse(
                exam.Id,
                exam.Title,
                exam.Description))
            .ToList();
    }
}
