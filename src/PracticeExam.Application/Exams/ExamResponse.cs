namespace PracticeExam.Application.Exams;

/// <summary>
/// Exam data exposed to API clients.
/// </summary>
public sealed record ExamResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
