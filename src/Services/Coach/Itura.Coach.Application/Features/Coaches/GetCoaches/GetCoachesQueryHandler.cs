using Itura.Coach.Application.DTOs;
using Itura.Coach.Application.Features.Coaches.GetCoach;
using Itura.Coach.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetCoaches;

internal sealed class GetCoachesQueryHandler(ICoachRepository repository)
    : IRequestHandler<GetCoachesQuery, Result<PagedResult<CoachDto>>>
{
    public async Task<Result<PagedResult<CoachDto>>> Handle(GetCoachesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetActiveAsync(
            request.Page, request.PageSize,
            request.Specialization, request.Language,
            request.MinRating, request.MaxHourlyRate,
            cancellationToken);

        var dtos = paged.Items.Select(GetCoachQueryHandler.ToDto).ToList();
        return Result.Success(new PagedResult<CoachDto>(dtos, paged.TotalCount, request.Page, request.PageSize));
    }
}
