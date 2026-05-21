using Microsoft.AspNetCore.Mvc;
using PracticeExam.Application.Exams;

namespace PracticeExam.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExamsController(
    GetExamsHandler getExamsHandler,
    GetExamByIdHandler getExamByIdHandler) : ControllerBase
{
    /// <summary>Returns every exam.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExamResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ExamResponse>>> GetExams(CancellationToken cancellationToken)
    {
        var exams = await getExamsHandler.HandleAsync(cancellationToken);

        return Ok(exams);
    }

    /// <summary>Returns a single exam with all of its questions and answers.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExamDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExamDetailResponse>> GetExamById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var exam = await getExamByIdHandler.HandleAsync(id, cancellationToken);

        return exam is null ? NotFound() : Ok(exam);
    }
}
