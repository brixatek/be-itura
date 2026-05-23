using Itura.Coach.Application.DTOs;
using Itura.Coach.Application.Features.Coaches.GetCoach;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.Verification;

internal sealed class GetPendingCoachesQueryHandler(ICoachRepository repository)
    : IRequestHandler<GetPendingCoachesQuery, Result<PagedResult<CoachDto>>>
{
    public async Task<Result<PagedResult<CoachDto>>> Handle(
        GetPendingCoachesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetPendingAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = paged.Items.Select(GetCoachQueryHandler.ToDto).ToList();
        return Result.Success(new PagedResult<CoachDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize));
    }
}
