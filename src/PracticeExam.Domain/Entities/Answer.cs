using PracticeExam.Domain.Common;

namespace PracticeExam.Domain.Entities;

/// <summary>
/// A candidate answer to a <see cref="Question"/>, flagged as correct or not.
/// </summary>
public sealed class Answer : Entity
{
    public Guid QuestionId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public bool IsCorrect { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    // Parameterless constructor for the persistence layer (EF Core) to materialize instances.
    private Answer()
    {
    }

    /// <summary>
    /// Reconstitutes an <see cref="Answer"/> from already-persisted values.
    /// </summary>
    public static Answer Create(
        Guid id,
        Guid questionId,
        string text,
        bool isCorrect,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Answer
        {
            Id = id,
            QuestionId = questionId,
            Text = text,
            IsCorrect = isCorrect,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
        };
    }
}
