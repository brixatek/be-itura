using Itura.AI.Application.Features.AI.Commands.GenerateRecommendations;
using Itura.AI.Application.Features.AI.Conversations;
using Itura.AI.Application.Features.AI.Journaling;
using Itura.AI.Application.Features.AI.Queries.GetRecommendations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Itura.AI.API.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public sealed class AIController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException());

    // ─── Recommendations ────────────────────────────────────────────────────────

    [HttpPost("recommendations")]
    public async Task<IActionResult> GenerateRecommendations(
        [FromBody] GenerateRecommendationsCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (result.IsFailure) return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("recommendations/me")]
    public async Task<IActionResult> GetMyRecommendations(
        [FromQuery] string? type, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRecommendationsQuery(GetUserId(), type, limit), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("recommendations/{userId:guid}")]
    public async Task<IActionResult> GetUserRecommendations(
        Guid userId, [FromQuery] string? type, [FromQuery] int limit = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRecommendationsQuery(userId, type, limit), ct);
        return Ok(new { success = true, data = result.Value });
    }

    // ─── Sera Conversations ─────────────────────────────────────────────────────

    [HttpGet("conversations")]
    public async Task<IActionResult> ListConversations([FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListConversationsQuery(GetUserId(), limit), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversation(string id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetConversationHistoryQuery(id, GetUserId()), ct);
        if (result.IsFailure) return NotFound(new { error = result.Error.Message });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("conversations/{id}")]
    public async Task<IActionResult> DeleteConversation(string id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteConversationCommand(id, GetUserId()), ct);
        if (result.IsFailure) return result.Error.Code.Contains("NotFound")
            ? NotFound(new { error = result.Error.Message })
            : StatusCode(403, new { error = result.Error.Message });
        return NoContent();
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var tier = User.FindFirstValue("tier") ?? "free";
        var result = await mediator.Send(new SendMessageCommand(GetUserId(), request.ConversationId, request.Message, tier), ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error.Message });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("conversations/stream")]
    public async Task StreamMessage([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        var tier = User.FindFirstValue("tier") ?? "free";
        var command = new StreamConversationCommand(GetUserId(), request.ConversationId, request.Message, tier);

        await foreach (var token in mediator.CreateStream(command, ct))
        {
            var line = $"data: {JsonSerializer.Serialize(new { token })}\n\n";
            await Response.WriteAsync(line, ct);
            await Response.Body.FlushAsync(ct);
        }

        await Response.WriteAsync("data: [DONE]\n\n", ct);
        await Response.Body.FlushAsync(ct);
    }

    // ─── Journal Prompts ────────────────────────────────────────────────────────

    [HttpGet("journal-prompts")]
    public async Task<IActionResult> GetJournalPrompts(CancellationToken ct)
    {
        var result = await mediator.Send(new GetJournalPromptsQuery(GetUserId()), ct);
        return Ok(new { success = true, data = result.Value });
    }
}

public sealed record SendMessageRequest(string Message, string? ConversationId = null);
