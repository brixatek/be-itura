using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

public sealed record TopUpWalletCommand(
    Guid UserId,
    decimal Amount,
    string Email,
    string? CallbackUrl = null) : IRequest<Result<PaystackPaymentInitDto>>;
