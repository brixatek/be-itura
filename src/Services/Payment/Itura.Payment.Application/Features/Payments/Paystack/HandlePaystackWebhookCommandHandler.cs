using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Application.Features.Wallet;
using Itura.Payment.Domain.Enums;
using Itura.Payment.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;
using System.Text.Json;

namespace Itura.Payment.Application.Features.Payments.Paystack;

internal sealed class HandlePaystackWebhookCommandHandler(
    IPaymentRepository repository,
    IWalletRepository walletRepository,
    IPaymentUnitOfWork unitOfWork,
    IPaystackService paystack,
    ISender sender)
    : IRequestHandler<HandlePaystackWebhookCommand, Result>
{
    public async Task<Result> Handle(HandlePaystackWebhookCommand request, CancellationToken cancellationToken)
    {
        if (!paystack.ValidateWebhookSignature(request.Payload, request.Signature))
            return Result.Failure(Error.Unauthorized("Invalid webhook signature."));

        using var doc = JsonDocument.Parse(request.Payload);
        var root = doc.RootElement;

        var eventType = root.GetProperty("event").GetString() ?? string.Empty;

        if (eventType != "charge.success")
            return Result.Success();

        var data = root.GetProperty("data");
        var reference = data.GetProperty("reference").GetString() ?? string.Empty;
        var amountInSmallestUnit = data.GetProperty("amount").GetInt64();
        var amount = amountInSmallestUnit / 100m;

        // Wallet top-up flow
        if (reference.StartsWith("topup_"))
        {
            // Extract userId from reference format: topup_{userId}_{timestamp}
            var parts = reference.Split('_');
            if (parts.Length >= 2 && Guid.TryParse(parts[1], out var userId))
            {
                var wallet = await walletRepository.GetByUserIdAsync(userId, cancellationToken);
                if (wallet is not null)
                {
                    await sender.Send(new CreditWalletCommand(userId, amount, "Wallet top-up via Paystack", reference), cancellationToken);
                }
            }
            return Result.Success();
        }

        // Standard payment flow
        var record = await repository.GetByReferenceAsync(reference, cancellationToken);
        if (record is null)
            return Result.Success();

        if (record.Status != PaymentStatus.Pending)
            return Result.Success();

        record.Complete(reference);
        repository.Update(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
