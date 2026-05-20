using Microsoft.AspNetCore.Mvc;
using PracticeExam.Application.Exams;

namespace PracticeExam.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExamsController(GetExamsHandler getExamsHandler) : ControllerBase
{
    /// <summary>Returns every exam.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExamResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ExamResponse>>> GetExams(CancellationToken cancellationToken)
    {
        var exams = await getExamsHandler.HandleAsync(cancellationToken);

        return Ok(exams);
    }
}
