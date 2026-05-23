using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.LikePost;

public sealed record LikePostCommand(Guid PostId, bool Unlike = false) : IRequest<Result>;
