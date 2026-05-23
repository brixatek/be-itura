using Itura.Content.Application.Features.Content.Commands.CreateContent;
using Itura.Content.Application.Features.Content.Commands.DeleteContent;
using Itura.Content.Application.Features.Content.Commands.PublishContent;
using Itura.Content.Application.Features.Content.Commands.UnpublishContent;
using Itura.Content.Application.Features.Content.Commands.UpdateContent;
using Itura.Content.Application.Features.Content.Queries.GetContent;
using Itura.Content.Application.Features.Content.Queries.GetContents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Content.API.Controllers;

[ApiController]
[Route("api/v1/contents")]
public sealed class ContentsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentRequest request, CancellationToken ct)
    {
        var command = new CreateContentCommand(
            request.Title,
            request.Description,
            request.Body,
            request.ContentType,
            GetUserId(),
            request.Tags,
            request.ThumbnailUrl,
            request.MediaUrl,
            request.DurationSeconds);

        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(nameof(GetContent), new { id = result.Value.Id },
            new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetContent(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetContentQuery(id), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetContents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? contentType = null,
        [FromQuery] string? tag = null,
        [FromQuery] Guid? authorUserId = null,
        [FromQuery] bool? isPublished = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetContentsQuery(page, pageSize, contentType, tag, authorUserId, isPublished), ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateContent(Guid id, [FromBody] UpdateContentRequest request, CancellationToken ct)
    {
        var command = new UpdateContentCommand(
            id,
            GetUserId(),
            request.Title,
            request.Description,
            request.Body,
            request.Tags,
            request.ThumbnailUrl,
            request.MediaUrl,
            request.DurationSeconds);

        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}/publish")]
    [Authorize]
    public async Task<IActionResult> PublishContent(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishContentCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpPut("{id:guid}/unpublish")]
    [Authorize]
    public async Task<IActionResult> UnpublishContent(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new UnpublishContentCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteContent(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteContentCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }
}

public sealed record CreateContentRequest(
    string Title,
    string? Description,
    string? Body,
    string ContentType,
    List<string>? Tags,
    string? ThumbnailUrl,
    string? MediaUrl,
    int? DurationSeconds);

public sealed record UpdateContentRequest(
    string Title,
    string? Description,
    string? Body,
    List<string>? Tags,
    string? ThumbnailUrl,
    string? MediaUrl,
    int? DurationSeconds);
