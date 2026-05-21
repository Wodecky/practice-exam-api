using Microsoft.EntityFrameworkCore;
using PracticeExam.Domain.Entities;

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
        modelBuilder.Entity<Exam>(exam =>
        {
            exam.ToTable("exams");

            exam.HasKey(e => e.Id);

            // The DB stores ids as lowercase 32-char hex (no dashes); map Guid <-> that form.
            exam.Property(e => e.Id)
                .HasColumnName("id")
                .HasConversion(
                    id => id.ToString("N"),
                    value => Guid.Parse(value));

            exam.Property(e => e.Title).HasColumnName("title");
            exam.Property(e => e.Description).HasColumnName("description");

            exam.HasMany(e => e.Questions)
                .WithOne()
                .HasForeignKey(q => q.ExamId);

            // Questions is exposed as a read-only property backed by a private field.
            exam.Navigation(e => e.Questions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Question>(question =>
        {
            question.ToTable("questions");

            question.HasKey(q => q.Id);

            question.Property(q => q.Id)
                .HasColumnName("id")
                .HasConversion(
                    id => id.ToString("N"),
                    value => Guid.Parse(value));

            question.Property(q => q.ExamId)
                .HasColumnName("exam_id")
                .HasConversion(
                    id => id.ToString("N"),
                    value => Guid.Parse(value));

            question.Property(q => q.Text).HasColumnName("text");

            question.HasMany(q => q.Answers)
                .WithOne()
                .HasForeignKey(a => a.QuestionId);

            question.Navigation(q => q.Answers)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Answer>(answer =>
        {
            answer.ToTable("answers");

            answer.HasKey(a => a.Id);

            answer.Property(a => a.Id)
                .HasColumnName("id")
                .HasConversion(
                    id => id.ToString("N"),
                    value => Guid.Parse(value));

            answer.Property(a => a.QuestionId)
                .HasColumnName("question_id")
                .HasConversion(
                    id => id.ToString("N"),
                    value => Guid.Parse(value));

            answer.Property(a => a.Text).HasColumnName("text");
            answer.Property(a => a.IsCorrect).HasColumnName("is_correct");
        });
    }
}
