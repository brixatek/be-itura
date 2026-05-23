using Itura.Mood.Application.Features.Mood.DeleteMoodEntry;
using Itura.Mood.Application.Features.Mood.GetHistory;
using Itura.Mood.Application.Features.Mood.GetSummary;
using Itura.Mood.Application.Features.Mood.LogMood;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Mood.API.Controllers;

[ApiController]
[Route("api/v1/mood")]
[Authorize]
public sealed class MoodController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpPost]
    public async Task<IActionResult> LogMood([FromBody] LogMoodRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new LogMoodCommand(
            CurrentUserId, request.Score, request.Note, request.Emotions ?? [], request.LoggedAt), ct);

        if (result.IsFailure) return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetHistory), new { }, new { id = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMoodHistoryQuery(CurrentUserId, page, pageSize, from, to), ct);
        return Ok(result.Value);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMoodSummaryQuery(CurrentUserId, days), ct);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteMoodEntryCommand(id, CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return NoContent();
    }
}

public sealed record LogMoodRequest(
    int Score,
    string? Note,
    IReadOnlyList<string>? Emotions,
    DateTime? LoggedAt);
