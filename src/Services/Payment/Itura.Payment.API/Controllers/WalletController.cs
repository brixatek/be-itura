using Itura.Payment.Application.Features.Wallet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Payment.API.Controllers;

[ApiController]
[Route("api/v1/wallet")]
[Authorize]
public sealed class WalletController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetWallet(CancellationToken ct)
    {
        var result = await mediator.Send(new GetWalletQuery(GetUserId()), ct);
        if (result.IsFailure) return NotFound(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new TopUpWalletCommand(
            GetUserId(), request.Amount, request.Email, request.CallbackUrl), ct);

        if (result.IsFailure) return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetWalletTransactionsQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record TopUpRequest(decimal Amount, string Email, string? CallbackUrl);
