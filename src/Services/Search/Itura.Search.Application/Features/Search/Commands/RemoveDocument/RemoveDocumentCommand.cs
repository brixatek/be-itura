using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Commands.RemoveDocument;

public sealed record RemoveDocumentCommand(string EntityType, Guid EntityId) : IRequest<Result>;
