using Itura.Media.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Queries.GetMedia;

public sealed record GetMediaQuery(Guid Id, Guid? RequestingUserId = null) : IRequest<Result<MediaAssetDto>>;
