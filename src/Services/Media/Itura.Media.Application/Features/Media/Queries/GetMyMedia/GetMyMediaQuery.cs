using Itura.Media.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Queries.GetMyMedia;

public sealed record GetMyMediaQuery(Guid UploaderUserId, int Page, int PageSize) : IRequest<Result<PagedResult<MediaAssetDto>>>;
