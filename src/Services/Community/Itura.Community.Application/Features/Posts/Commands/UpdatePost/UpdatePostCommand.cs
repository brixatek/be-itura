using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.UpdatePost;

public sealed record UpdatePostCommand(
    Guid Id,
    Guid AuthorUserId,
    string Body,
    string? Title,
    List<string>? Tags) : IRequest<Result<CommunityPostDto>>;
