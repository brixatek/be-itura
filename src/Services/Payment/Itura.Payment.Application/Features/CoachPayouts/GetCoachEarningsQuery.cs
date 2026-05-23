using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

public sealed record GetCoachEarningsQuery(Guid CoachUserId) : IRequest<Result<CoachEarningsSummaryDto>>;
