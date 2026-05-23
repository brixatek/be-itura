using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.UnpublishContent;

public sealed record UnpublishContentCommand(Guid Id, Guid AuthorUserId) : IRequest<Result>;
