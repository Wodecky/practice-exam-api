using Microsoft.EntityFrameworkCore;
using PracticeExam.Application.Common.Interfaces;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Infrastructure.Persistence;

/// <summary>
/// EF Core-backed <see cref="IExamRepository"/>.
/// </summary>
public sealed class ExamRepository(PracticeExamDbContext dbContext) : IExamRepository
{
    public async Task<IReadOnlyList<Exam>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Exams
            .AsNoTracking()
            .OrderBy(exam => exam.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
