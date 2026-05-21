using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeExam.Domain.Entities;

namespace PracticeExam.Infrastructure.Persistence.Configurations;

internal sealed class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> answer)
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
    }
}
