using PracticeExam.Domain.Common;

namespace PracticeExam.Domain.Entities;

/// <summary>
/// A practice exam: a titled collection of questions.
/// </summary>
public sealed class Exam : Entity
{
    private readonly List<Question> _questions = [];

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    /// <summary>The exam's questions.</summary>
    public IReadOnlyList<Question> Questions => _questions;

    // Parameterless constructor for the persistence layer (EF Core) to materialize instances.
    private Exam()
    {
    }

    /// <summary>
    /// Reconstitutes an <see cref="Exam"/> from already-persisted values
    /// (its <paramref name="id"/> is supplied, not generated).
    /// </summary>
    public static Exam Create(
        Guid id,
        string title,
        string? description,
        IEnumerable<Question>? questions = null)
    {
        var exam = new Exam
        {
            Id = id,
            Title = title,
            Description = description,
        };

        if (questions is not null)
        {
            exam._questions.AddRange(questions);
        }

        return exam;
    }
}
