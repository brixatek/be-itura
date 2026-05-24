using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.CoachPayouts;

internal sealed class ProcessPayoutBatchCommandHandler(
    ICoachPayoutRepository repository,
    IPaymentUnitOfWork unitOfWork,
    IPaystackService paystack,
    IFieldEncryptionService encryption)
    : IRequestHandler<ProcessPayoutBatchCommand, Result<ProcessPayoutBatchResult>>
{
    public async Task<Result<ProcessPayoutBatchResult>> Handle(
        ProcessPayoutBatchCommand request, CancellationToken cancellationToken)
    {
        var coachIds = await repository.GetCoachesWithUnpaidEarningsAsync(cancellationToken);
        var processed = 0;
        var failed = 0;
        var totalAmount = 0m;

        foreach (var coachId in coachIds)
        {
            try
            {
                var bankAccount = await repository.GetBankAccountAsync(coachId, cancellationToken);
                if (bankAccount is null) { failed++; continue; }

                var unpaidAmount = await repository.GetUnpaidEarningsTotalAsync(coachId, cancellationToken);
                if (unpaidAmount < 1000m) continue; // minimum payout threshold: 1000 NGN

                var bankCode = encryption.Decrypt(bankAccount.BankCodeEncrypted);
                var accountNumber = encryption.Decrypt(bankAccount.AccountNumberEncrypted);
                var accountName = encryption.Decrypt(bankAccount.AccountNameEncrypted);

                // Create transfer recipient in Paystack
                var recipientResult = await paystack.CreateTransferRecipientAsync(
                    bankCode, accountNumber, accountName, cancellationToken);

                if (!recipientResult.Success || recipientResult.TransferCode is null)
                { failed++; continue; }

                var payout = CoachPayout.Create(coachId, unpaidAmount);
                await repository.AddPayoutAsync(payout, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var transferReference = $"payout_{payout.Id:N}";
                var amountInKobo = (long)(unpaidAmount * 100);

                var transferResult = await paystack.InitiateTransferAsync(
                    recipientResult.TransferCode, amountInKobo, transferReference, cancellationToken);

                if (!transferResult.Success)
                {
                    payout.MarkFailed(transferResult.Message ?? "Transfer failed");
                    repository.UpdatePayout(payout);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    failed++;
                    continue;
                }

                payout.MarkProcessing(transferReference);
                repository.UpdatePayout(payout);

                // Mark all unpaid earnings as paid
                var earnings = await repository.GetUnpaidEarningsAsync(coachId, cancellationToken);
                foreach (var earning in earnings)
                {
                    earning.MarkPaid(payout.Id);
                    repository.UpdateEarning(earning);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                processed++;
                totalAmount += unpaidAmount;
            }
            catch
            {
                failed++;
            }
        }

        return Result.Success(new ProcessPayoutBatchResult(processed, failed, totalAmount));
    }
}
