using Itura.Payment.Application.Features.CoachPayouts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Payment.API.Controllers;

[ApiController]
[Route("api/v1/coaches/me")]
[Authorize]
public sealed class CoachesController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("earnings")]
    public async Task<IActionResult> GetEarnings(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCoachEarningsQuery(GetUserId()), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("payouts")]
    public async Task<IActionResult> GetPayouts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCoachPayoutsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("bank-account")]
    public async Task<IActionResult> SaveBankAccount([FromBody] SaveBankAccountRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new SaveBankAccountCommand(
            GetUserId(), request.BankCode, request.AccountNumber, request.AccountName, request.BankName), ct);

        if (result.IsFailure) return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true });
    }
}

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminPayoutsController(IMediator mediator) : ControllerBase
{
    [HttpPost("payouts/process")]
    public async Task<IActionResult> ProcessPayouts(CancellationToken ct)
    {
        var result = await mediator.Send(new ProcessPayoutBatchCommand(), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record SaveBankAccountRequest(
    string BankCode,
    string AccountNumber,
    string AccountName,
    string BankName);
