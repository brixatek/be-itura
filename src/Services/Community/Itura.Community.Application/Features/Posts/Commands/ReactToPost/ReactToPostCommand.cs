using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.ReactToPost;

public sealed record ReactToPostCommand(Guid PostId, Guid UserId, string Emoji, bool Remove = false) : IRequest<Result>;
