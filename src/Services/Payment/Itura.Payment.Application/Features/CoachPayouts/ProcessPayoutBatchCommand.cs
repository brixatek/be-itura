using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

public sealed record ProcessPayoutBatchCommand() : IRequest<Result<ProcessPayoutBatchResult>>;

public sealed record ProcessPayoutBatchResult(int Processed, int Failed, decimal TotalNetAmount, decimal TotalCommission);
