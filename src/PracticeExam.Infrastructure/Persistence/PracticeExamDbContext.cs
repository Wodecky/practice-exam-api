using Microsoft.EntityFrameworkCore;
using PracticeExam.Domain.Entities;
using PracticeExam.Infrastructure.Persistence.Configurations;

namespace PracticeExam.Infrastructure.Persistence;

/// <summary>
/// EF Core context over the sqitch-managed <c>practice_exam.db</c> SQLite database.
/// The schema is owned externally — this context only reads it.
/// </summary>
public sealed class PracticeExamDbContext(DbContextOptions<PracticeExamDbContext> options)
    : DbContext(options)
{
    public DbSet<Exam> Exams => Set<Exam>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new AnswerConfiguration());
    }
}
