using PracticeExam.Domain.Entities;

namespace PracticeExam.Application.Common.Interfaces;

/// <summary>
/// Read access to the <see cref="Exam"/> store. Implemented by the Infrastructure layer.
/// </summary>
public interface IExamRepository
{
    /// <summary>Returns every exam, ordered by creation time.</summary>
    Task<IReadOnlyList<Exam>> GetAllAsync(CancellationToken cancellationToken = default);
}
