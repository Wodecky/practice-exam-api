using PracticeExam.Domain.Common;

namespace PracticeExam.Domain.Entities;

/// <summary>
/// A practice exam: a titled collection of questions.
/// </summary>
public sealed class Exam : Entity
{
    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    // Parameterless constructor for the persistence layer (EF Core) to materialize instances.
    private Exam()
    {
    }

    /// <summary>
    /// Reconstitutes an <see cref="Exam"/> from already-persisted values
    /// (its <paramref name="id"/> and timestamps are supplied, not generated).
    /// </summary>
    public static Exam Create(
        Guid id,
        string title,
        string? description,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Exam
        {
            Id = id,
            Title = title,
            Description = description,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };
    }
}
