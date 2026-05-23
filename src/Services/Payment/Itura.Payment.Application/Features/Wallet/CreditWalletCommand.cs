using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

public sealed record CreditWalletCommand(
    Guid UserId,
    decimal Amount,
    string Description,
    string? Reference = null) : IRequest<Result>;
