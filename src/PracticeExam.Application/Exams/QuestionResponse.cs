namespace PracticeExam.Application.Exams;

/// <summary>A question within an <see cref="ExamDetailResponse"/>.</summary>
public sealed record QuestionResponse(
    Guid Id,
    string Text,
    IReadOnlyList<AnswerResponse> Answers);
