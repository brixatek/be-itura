using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.CreatePost;

public sealed record CreatePostCommand(
    string Body,
    string PostType,
    Guid AuthorUserId,
    string? Title,
    List<string>? Tags,
    bool IsPublic) : IRequest<Result<CommunityPostDto>>;
