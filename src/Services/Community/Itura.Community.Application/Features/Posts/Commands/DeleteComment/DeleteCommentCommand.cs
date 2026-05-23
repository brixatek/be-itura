using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.DeleteComment;

public sealed record DeleteCommentCommand(Guid CommentId, Guid AuthorUserId) : IRequest<Result>;
