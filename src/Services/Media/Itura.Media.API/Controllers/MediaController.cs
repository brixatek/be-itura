using Itura.Media.Application.Features.Media.Commands.DeleteMedia;
using Itura.Media.Application.Features.Media.Commands.UploadMedia;
using Itura.Media.Application.Features.Media.Queries.GetMedia;
using Itura.Media.Application.Features.Media.Queries.GetMyMedia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Media.API.Controllers;

[ApiController]
[Route("api/v1/media")]
public sealed class MediaController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string? mediaType,
        [FromForm] bool isPublic = true,
        CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { success = false, error = new { code = "Media.NoFile", message = "No file provided." } });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        var command = new UploadMediaCommand(
            file.FileName,
            file.ContentType,
            file.Length,
            mediaType ?? InferMediaType(file.ContentType),
            isPublic,
            GetUserId(),
            ms.ToArray());

        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(nameof(GetMedia), new { id = result.Value.Id },
            new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMedia(Guid id, CancellationToken ct)
    {
        Guid? userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;
        var result = await mediator.Send(new GetMediaQuery(id, userId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMyMedia(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMyMediaQuery(GetUserId(), page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteMedia(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteMediaCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }

    private static string InferMediaType(string mimeType) => mimeType switch
    {
        var t when t.StartsWith("image/") => "Image",
        var t when t.StartsWith("video/") => "Video",
        var t when t.StartsWith("audio/") => "Audio",
        _ => "Document"
    };
}
