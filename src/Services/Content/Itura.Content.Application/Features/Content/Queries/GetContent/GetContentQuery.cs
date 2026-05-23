using Itura.Content.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Queries.GetContent;

public sealed record GetContentQuery(Guid Id) : IRequest<Result<ContentItemDto>>;
