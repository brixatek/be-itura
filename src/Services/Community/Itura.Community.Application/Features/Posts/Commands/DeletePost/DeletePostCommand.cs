using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.DeletePost;

public sealed record DeletePostCommand(Guid Id, Guid AuthorUserId) : IRequest<Result>;
