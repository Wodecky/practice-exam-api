using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Infrastructure.Persistence.Configurations;

internal sealed class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> exam)
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

        exam.HasMany(e => e.Questions)
            .WithOne()
            .HasForeignKey(q => q.ExamId);

        // Questions is exposed as a read-only property backed by a private field.
        exam.Navigation(e => e.Questions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
