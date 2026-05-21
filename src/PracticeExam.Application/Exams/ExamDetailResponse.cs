namespace PracticeExam.Application.Exams;

/// <summary>
/// A single exam together with all of its questions and answers.
/// </summary>
public sealed record ExamDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<QuestionResponse> Questions);

/// <summary>A question within an <see cref="ExamDetailResponse"/>.</summary>
public sealed record QuestionResponse(
    Guid Id,
    string Text,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<AnswerResponse> Answers);

/// <summary>A candidate answer within a <see cref="QuestionResponse"/>.</summary>
public sealed record AnswerResponse(
    Guid Id,
    string Text,
    bool IsCorrect,
    DateTime CreatedAt,
    DateTime UpdatedAt);
