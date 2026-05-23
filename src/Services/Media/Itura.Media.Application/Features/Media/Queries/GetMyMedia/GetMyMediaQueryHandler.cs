using Itura.Media.Application.DTOs;
using Itura.Media.Application.Features.Media.Queries.GetMedia;
using Itura.Media.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Queries.GetMyMedia;

internal sealed class GetMyMediaQueryHandler(IMediaAssetRepository repository)
    : IRequestHandler<GetMyMediaQuery, Result<PagedResult<MediaAssetDto>>>
{
    public async Task<Result<PagedResult<MediaAssetDto>>> Handle(GetMyMediaQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByUploaderUserIdAsync(
            request.UploaderUserId, request.Page, request.PageSize, cancellationToken);

        var dtos = paged.Items.Select(GetMediaQueryHandler.ToDto).ToList();
        return new PagedResult<MediaAssetDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
