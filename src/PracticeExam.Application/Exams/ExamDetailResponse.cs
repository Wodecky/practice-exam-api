namespace PracticeExam.Application.Exams;

/// <summary>
/// A single exam together with all of its questions and answers.
/// </summary>
public sealed record ExamDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    IReadOnlyList<QuestionResponse> Questions);
