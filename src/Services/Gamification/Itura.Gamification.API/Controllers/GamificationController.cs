using Itura.Gamification.Application.Features.Gamification.Commands.AwardPoints;
using Itura.Gamification.Application.Features.Gamification.Queries.GetLeaderboard;
using Itura.Gamification.Application.Features.Gamification.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Gamification.API.Controllers;

[ApiController]
[Route("api/v1/gamification")]
public sealed class GamificationController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("points")]
    [Authorize]
    public async Task<IActionResult> AwardPoints([FromBody] AwardPointsRequest request, CancellationToken ct)
    {
        var command = new AwardPointsCommand(request.UserId, request.Points, request.Reason);
        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("profile/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetProfile(Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProfileQuery(userId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetProfileQuery(GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int top = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetLeaderboardQuery(top), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record AwardPointsRequest(Guid UserId, int Points, string Reason);
