using CheckinProjectBackend.Application.Dtos;
using CheckinProjectBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CheckinProjectBackend.Controllers;

[ApiController]
[Route("work")]
public class WorkController : ControllerBase
{
    private readonly IWorkService _workService;

    public WorkController(IWorkService workService)
    {
        _workService = workService;
    }

    public sealed record CheckRequest(int EmployeeId);

    [HttpPost("checkin")]
    public async Task<ActionResult<WorkRegisterDto>> CheckIn(
        [FromBody] CheckRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workService.CheckInAsync(request.EmployeeId, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<WorkRegisterDto>> CheckOut(
        [FromBody] CheckRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _workService.CheckOutAsync(request.EmployeeId, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<WorkRegisterDto>>> List(
        [FromQuery] string? name,
        [FromQuery] DateTime? date,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _workService.ListAsync(name, date, page, pageSize, cancellationToken);
        return Ok(result);
    }
}
