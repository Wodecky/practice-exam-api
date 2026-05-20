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
            exam.Property(e => e.CreatedAt).HasColumnName("created_at");
            exam.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });
    }
}
