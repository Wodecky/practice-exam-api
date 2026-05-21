using PracticeExam.Domain.Common;

namespace PracticeExam.Domain.Entities;

/// <summary>
/// A single question belonging to an <see cref="Exam"/>, with its candidate answers.
/// </summary>
public sealed class Question : Entity
{
    private readonly List<Answer> _answers = [];

    public Guid ExamId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    /// <summary>The question's candidate answers.</summary>
    public IReadOnlyList<Answer> Answers => _answers;

    // Parameterless constructor for the persistence layer (EF Core) to materialize instances.
    private Question()
    {
    }

    /// <summary>
    /// Reconstitutes a <see cref="Question"/> from already-persisted values.
    /// </summary>
    public static Question Create(
        Guid id,
        Guid examId,
        string text,
        IEnumerable<Answer>? answers = null)
    {
        var question = new Question
        {
            Id = id,
            ExamId = examId,
            Text = text,
        };

        if (answers is not null)
        {
            question._answers.AddRange(answers);
        }

        return question;
    }
}
