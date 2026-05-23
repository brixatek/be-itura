using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

internal sealed class GetCoachPayoutsQueryHandler(ICoachPayoutRepository repository)
    : IRequestHandler<GetCoachPayoutsQuery, Result<PagedResult<CoachPayoutDto>>>
{
    public async Task<Result<PagedResult<CoachPayoutDto>>> Handle(
        GetCoachPayoutsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetPayoutsAsync(
            request.CoachUserId, request.Page, request.PageSize, cancellationToken);

        var dtos = paged.Items.Select(p => new CoachPayoutDto(
            p.Id, p.Amount, p.Currency, p.Status,
            p.TransferReference, p.FailureReason, p.ProcessedAt, p.CreatedAt)).ToList();

        return Result.Success(new PagedResult<CoachPayoutDto>(
            dtos, paged.TotalCount, paged.Page, paged.PageSize));
    }
}
