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
            .OrderBy(exam => exam.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Exam?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Split query: the two nested one-to-many includes would otherwise produce a
        // cartesian product (questions x answers) in a single join. OrderBy keeps the
        // result deterministic across the split queries' limiting operator (First).
        return await dbContext.Exams
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(exam => exam.Id)
            .Include(exam => exam.Questions)
            .ThenInclude(question => question.Answers)
            .FirstOrDefaultAsync(exam => exam.Id == id, cancellationToken);
    }
}
