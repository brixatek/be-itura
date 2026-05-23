using Itura.Auth.Application.Features.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itura.Auth.API.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "RequireAdmin")]
public sealed class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("dashboard/overview")]
    public async Task<IActionResult> GetOverview(CancellationToken ct)
    {
        var result = await mediator.Send(new GetDashboardOverviewQuery(), ct);
        if (result.IsFailure)
            return StatusCode(500, new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? role = null,
        [FromQuery] DateTime? registeredFrom = null,
        [FromQuery] DateTime? registeredTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new SearchAccountsQuery(search, status, role, registeredFrom, registeredTo, page, pageSize), ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("users/{id:guid}/suspend")]
    public async Task<IActionResult> SuspendUser(Guid id, [FromBody] SuspendUserRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new SuspendAccountCommand(id, request.Reason), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpPost("users/{id:guid}/restore")]
    public async Task<IActionResult> RestoreUser(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new RestoreAccountCommand(id), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpGet("coaches/pending")]
    public IActionResult GetPendingCoaches()
    {
        // Stub: coach verification queue lives in the Coach service
        return Ok(new { success = true, data = new { items = Array.Empty<object>(), totalCount = 0 } });
    }

    [HttpGet("moderation/queue")]
    public IActionResult GetModerationQueue()
    {
        // Stub: moderation queue lives in the Community service
        return Ok(new { success = true, data = new { items = Array.Empty<object>(), totalCount = 0 } });
    }

    [HttpPost("moderation/{id:guid}/action")]
    public IActionResult ModerationAction(Guid id, [FromBody] ModerationActionRequest request)
    {
        // Stub: moderation action lives in the Community service
        return Ok(new { success = true });
    }

    [HttpGet("payments/overview")]
    public IActionResult GetPaymentsOverview()
    {
        // Stub: payment data lives in the Payment service
        return Ok(new
        {
            success = true,
            data = new
            {
                totalRevenue = 0m,
                totalRefunds = 0m,
                pendingPayouts = 0m,
                currency = "USD"
            }
        });
    }

    [HttpGet("analytics/revenue")]
    public IActionResult GetRevenueAnalytics(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        // Stub: revenue analytics live in the Analytics/Payment service
        return Ok(new
        {
            success = true,
            data = new
            {
                subscriptionMrr = 0m,
                sessionRevenue = 0m,
                refunds = 0m,
                period = new { from, to },
                currency = "USD"
            }
        });
    }
}

public sealed record SuspendUserRequest(string? Reason);
public sealed record ModerationActionRequest(string Action, string? Notes);
