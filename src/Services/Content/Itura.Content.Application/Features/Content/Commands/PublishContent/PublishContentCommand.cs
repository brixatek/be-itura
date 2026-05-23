using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.PublishContent;

public sealed record PublishContentCommand(Guid Id, Guid AuthorUserId) : IRequest<Result>;
