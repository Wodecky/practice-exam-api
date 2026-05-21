namespace PracticeExam.Application.Exams;

/// <summary>A candidate answer within a <see cref="QuestionResponse"/>.</summary>
public sealed record AnswerResponse(
    Guid Id,
    string Text,
    bool IsCorrect);
