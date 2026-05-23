using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Commands.DeleteMedia;

public sealed record DeleteMediaCommand(Guid Id, Guid RequestingUserId) : IRequest<Result>;
