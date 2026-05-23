using Itura.Payment.Application.DTOs;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

internal sealed class GetCoachEarningsQueryHandler(ICoachPayoutRepository repository)
    : IRequestHandler<GetCoachEarningsQuery, Result<CoachEarningsSummaryDto>>
{
    public async Task<Result<CoachEarningsSummaryDto>> Handle(
        GetCoachEarningsQuery request, CancellationToken cancellationToken)
    {
        var total = await repository.GetTotalEarningsAsync(request.CoachUserId, cancellationToken);
        var unpaid = await repository.GetUnpaidEarningsTotalAsync(request.CoachUserId, cancellationToken);
        return Result.Success(new CoachEarningsSummaryDto(total, unpaid, "NGN"));
    }
}
