using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

public sealed record SaveBankAccountCommand(
    Guid CoachUserId,
    string BankCode,
    string AccountNumber,
    string AccountName,
    string BankName) : IRequest<Result>;
