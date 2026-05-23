using Itura.Journal.Application.Features.Journal.CoachSharing;
using Itura.Journal.Application.Features.Journal.CreateEntry;
using Itura.Journal.Application.Features.Journal.DeleteEntry;
using Itura.Journal.Application.Features.Journal.GetEntries;
using Itura.Journal.Application.Features.Journal.GetEntry;
using Itura.Journal.Application.Features.Journal.GetTags;
using Itura.Journal.Application.Features.Journal.UpdateEntry;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Journal.API.Controllers;

[ApiController]
[Route("api/v1/journal")]
[Authorize]
public sealed class JournalController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpPost]
    public async Task<IActionResult> CreateEntry([FromBody] CreateJournalEntryRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateJournalEntryCommand(
            CurrentUserId, request.Title, request.Content ?? string.Empty,
            request.Tags ?? [], request.MoodScore, request.IsPrivate), ct);

        if (result.IsFailure) return BadRequest(new { error = result.Error.Message });
        return CreatedAtAction(nameof(GetEntry), new { id = result.Value }, new { id = result.Value });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEntry(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetJournalEntryQuery(id, CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetEntries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tag = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetJournalEntriesQuery(CurrentUserId, page, pageSize, tag, from, to), ct);
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEntry(Guid id, [FromBody] UpdateJournalEntryRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateJournalEntryCommand(
            id, CurrentUserId, request.Title, request.Content ?? string.Empty,
            request.Tags ?? [], request.MoodScore, request.IsPrivate), ct);

        if (result.IsFailure) return result.Error.Code.StartsWith("Auth.")
            ? Forbid()
            : NotFound(new { error = result.Error.Message });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEntry(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteJournalEntryCommand(id, CurrentUserId), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return NoContent();
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetJournalTagsQuery(CurrentUserId), ct);
        return Ok(result.Value);
    }

    [HttpPost("entries/{entryId:guid}/share/{coachId:guid}")]
    public async Task<IActionResult> Share(Guid entryId, Guid coachId, CancellationToken ct)
    {
        var result = await sender.Send(new ShareJournalEntryCommand(entryId, CurrentUserId, coachId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.StartsWith("Auth")) return Forbid();
            return NotFound(new { error = result.Error.Message });
        }
        return Ok(new { success = true });
    }

    [HttpDelete("entries/{entryId:guid}/share/{coachId:guid}")]
    public async Task<IActionResult> Unshare(Guid entryId, Guid coachId, CancellationToken ct)
    {
        var result = await sender.Send(new UnshareJournalEntryCommand(entryId, CurrentUserId, coachId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.StartsWith("Auth")) return Forbid();
            return NotFound(new { error = result.Error.Message });
        }
        return NoContent();
    }

    [HttpGet("shared-with-me")]
    public async Task<IActionResult> GetSharedWithMe(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCoachSharedJournalsQuery(CurrentUserId, page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record CreateJournalEntryRequest(
    string Title,
    string? Content,
    IReadOnlyList<string>? Tags,
    int? MoodScore,
    bool IsPrivate = false);

public sealed record UpdateJournalEntryRequest(
    string Title,
    string? Content,
    IReadOnlyList<string>? Tags,
    int? MoodScore,
    bool IsPrivate = false);
