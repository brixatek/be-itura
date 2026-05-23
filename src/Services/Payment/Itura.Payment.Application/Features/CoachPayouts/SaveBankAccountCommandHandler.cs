using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

internal sealed class SaveBankAccountCommandHandler(
    ICoachPayoutRepository repository,
    IPaymentUnitOfWork unitOfWork,
    IFieldEncryptionService encryption)
    : IRequestHandler<SaveBankAccountCommand, Result>
{
    public async Task<Result> Handle(SaveBankAccountCommand request, CancellationToken cancellationToken)
    {
        var encBankCode = encryption.Encrypt(request.BankCode);
        var encAccountNumber = encryption.Encrypt(request.AccountNumber);
        var encAccountName = encryption.Encrypt(request.AccountName);

        var existing = await repository.GetBankAccountAsync(request.CoachUserId, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateDetails(encBankCode, encAccountNumber, encAccountName, request.BankName);
            repository.UpdateBankAccount(existing);
        }
        else
        {
            var account = CoachBankAccount.Create(
                request.CoachUserId, encBankCode, encAccountNumber, encAccountName, request.BankName);
            await repository.AddBankAccountAsync(account, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
