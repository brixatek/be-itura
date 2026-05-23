using Itura.Search.Application.Features.Search.Commands.IndexDocument;
using Itura.Search.Application.Features.Search.Commands.RemoveDocument;
using Itura.Search.Application.Features.Search.Queries.SearchDocuments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itura.Search.API.Controllers;

[ApiController]
[Route("api/v1/search")]
public sealed class SearchController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string? entityType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { success = false, error = new { code = "Search.EmptyQuery", message = "Query cannot be empty." } });

        var result = await mediator.Send(new SearchDocumentsQuery(q, entityType, page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("index")]
    [Authorize]
    public async Task<IActionResult> Index([FromBody] IndexDocumentCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{entityType}/{entityId:guid}")]
    [Authorize]
    public async Task<IActionResult> Remove(string entityType, Guid entityId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveDocumentCommand(entityType, entityId), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }
}
