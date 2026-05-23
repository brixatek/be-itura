using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.DeleteContent;

public sealed record DeleteContentCommand(Guid Id, Guid AuthorUserId) : IRequest<Result>;
