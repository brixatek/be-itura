using Itura.Community.Application.Features.Posts.Commands.CreateComment;
using Itura.Community.Application.Features.Posts.Commands.CreatePost;
using Itura.Community.Application.Features.Posts.Commands.DeleteComment;
using Itura.Community.Application.Features.Posts.Commands.DeletePost;
using Itura.Community.Application.Features.Posts.Commands.LikePost;
using Itura.Community.Application.Features.Posts.Commands.UpdatePost;
using Itura.Community.Application.Features.Posts.Queries.GetFeed;
using Itura.Community.Application.Features.Posts.Queries.GetHotFeed;
using Itura.Community.Application.Features.Posts.Queries.GetPost;
using Itura.Community.Application.Features.Posts.Queries.GetPostComments;
using Itura.Community.Application.Features.Posts.Queries.GetPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Itura.Community.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
public sealed class PostsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreatePostCommand(
            request.Body, request.PostType, GetUserId(), request.Title, request.Tags, request.IsPublic), ct);

        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });

        return CreatedAtAction(nameof(GetPost), new { id = result.Value.Id },
            new { success = true, data = result.Value });
    }

    [HttpGet("feed")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeed(
        [FromQuery] string? cursor = null,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? postType = null,
        [FromQuery] string? tag = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetFeedQuery(cursor, pageSize, postType, tag), ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("hot")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHotFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int windowDays = 7,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetHotFeedQuery(page, pageSize, windowDays), ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? postType = null,
        [FromQuery] string? tag = null,
        [FromQuery] Guid? authorUserId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPostsQuery(page, pageSize, postType, tag, authorUserId), ct);
        if (result.IsFailure)
            return BadRequest(new { success = false, error = result.Error });
        return Ok(new { success = true, data = result.Value });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPostQuery(id), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdatePostCommand(id, GetUserId(), request.Body, request.Title, request.Tags), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true, data = result.Value });
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeletePostCommand(id, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> LikePost(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new LikePostCommand(id), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpDelete("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> UnlikePost(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new LikePostCommand(id, Unlike: true), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return Ok(new { success = true });
    }

    [HttpGet("{id:guid}/comments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetComments(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPostCommentsQuery(id, page, pageSize), ct);
        return Ok(new { success = true, data = result.Value });
    }

    [HttpPost("{id:guid}/comments")]
    [Authorize]
    public async Task<IActionResult> CreateComment(Guid id, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateCommentCommand(id, GetUserId(), request.Body), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            return BadRequest(new { success = false, error = result.Error });
        }
        return StatusCode(201, new { success = true, data = result.Value });
    }

    [HttpDelete("{postId:guid}/comments/{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteCommentCommand(commentId, GetUserId()), ct);
        if (result.IsFailure)
        {
            if (result.Error.Code.EndsWith("NotFound")) return NotFound(new { success = false, error = result.Error });
            if (result.Error.Code == "Auth.Forbidden") return Forbid();
            return BadRequest(new { success = false, error = result.Error });
        }
        return NoContent();
    }
}

public sealed record CreatePostRequest(string Body, string PostType, string? Title, List<string>? Tags, bool IsPublic = true);
public sealed record UpdatePostRequest(string Body, string? Title, List<string>? Tags);
public sealed record CreateCommentRequest(string Body);
