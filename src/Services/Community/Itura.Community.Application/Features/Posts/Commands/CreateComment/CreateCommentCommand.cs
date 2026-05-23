using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.CreateComment;

public sealed record CreateCommentCommand(
    Guid PostId,
    Guid AuthorUserId,
    string Body) : IRequest<Result<PostCommentDto>>;
