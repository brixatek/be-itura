using Itura.Coach.Application.Features.Coaches.CreateProfile;
using Itura.Coach.Application.Features.Coaches.DeactivateCoach;
using Itura.Coach.Application.Features.Coaches.GetCoach;
using Itura.Coach.Application.Features.Coaches.GetCoaches;
using Itura.Coach.Application.Features.Coaches.GetMyProfile;
using Itura.Coach.Application.Features.Coaches.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Coach.API.Controllers;

[ApiController]
[Route("api/v1/coaches")]
[Authorize]
public sealed class CoachesController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoaches(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? specialization = null,
        [FromQuery] string? language = null,
        [FromQuery] double? minRating = null,
        [FromQuery] decimal? maxHourlyRate = null,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachesQuery(page, pageSize, specialization, language, minRating, maxHourlyRate), ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoach(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachQuery(id), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMyCoachProfileQuery(CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateCoachProfileRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateCoachProfileCommand(
            CurrentUserId, request.DisplayName, request.Bio ?? string.Empty,
            request.Specializations ?? [], request.Languages ?? ["english"],
            request.HourlyRate, request.Currency ?? "USD",
            request.ProfileImageUrl, request.YearsOfExperience), ct);

        if (result.IsFailure) return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetCoach), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateCoachProfileRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateCoachProfileCommand(
            id, CurrentUserId, request.DisplayName, request.Bio ?? string.Empty,
            request.Specializations ?? [], request.Languages ?? ["english"],
            request.HourlyRate, request.Currency ?? "USD",
            request.ProfileImageUrl, request.YearsOfExperience), ct);

        if (result.IsFailure) return result.Error.Code.StartsWith("Auth.")
            ? Forbid()
            : NotFound(new { error = result.Error.Message });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeactivateCoachCommand(id, CurrentUserId), ct);
        if (result.IsFailure) return result.Error.Code.StartsWith("Auth.")
            ? Forbid()
            : NotFound(new { error = result.Error.Message });

        return NoContent();
    }
}

public sealed record CreateCoachProfileRequest(
    string DisplayName,
    string? Bio,
    IReadOnlyList<string>? Specializations,
    IReadOnlyList<string>? Languages,
    decimal HourlyRate,
    string? Currency,
    string? ProfileImageUrl,
    int YearsOfExperience);

public sealed record UpdateCoachProfileRequest(
    string DisplayName,
    string? Bio,
    IReadOnlyList<string>? Specializations,
    IReadOnlyList<string>? Languages,
    decimal HourlyRate,
    string? Currency,
    string? ProfileImageUrl,
    int YearsOfExperience);
