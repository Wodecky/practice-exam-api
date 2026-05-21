using PracticeExam.Application.Common.Interfaces;

namespace PracticeExam.Application.Exams;

/// <summary>
/// Use case: fetch a single exam with all of its questions and answers.
/// </summary>
public sealed class GetExamByIdHandler(IExamRepository examRepository)
{
    /// <summary>
    /// Returns the exam with the given <paramref name="id"/>, or <c>null</c>
    /// when no such exam exists.
    /// </summary>
    public async Task<ExamDetailResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var exam = await examRepository.GetByIdAsync(id, cancellationToken);

        if (exam is null)
        {
            return null;
        }

        return new ExamDetailResponse(
            exam.Id,
            exam.Title,
            exam.Description,
            exam.CreatedAt,
            exam.UpdatedAt,
            exam.Questions
                .OrderBy(question => question.CreatedAt)
                .Select(question => new QuestionResponse(
                    question.Id,
                    question.Text,
                    question.CreatedAt,
                    question.UpdatedAt,
                    question.Answers
                        .OrderBy(answer => answer.CreatedAt)
                        .Select(answer => new AnswerResponse(
                            answer.Id,
                            answer.Text,
                            answer.IsCorrect,
                            answer.CreatedAt,
                            answer.UpdatedAt))
                        .ToList()))
                .ToList());
    }
}
