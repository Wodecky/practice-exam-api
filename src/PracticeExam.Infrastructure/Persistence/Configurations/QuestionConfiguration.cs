using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Infrastructure.Persistence.Configurations;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> question)
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
    }
}
