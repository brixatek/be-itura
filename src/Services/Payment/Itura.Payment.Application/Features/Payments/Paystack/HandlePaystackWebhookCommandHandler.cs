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
    ICoachPayoutRepository coachPayoutRepository,
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
        var data = root.GetProperty("data");

        return eventType switch
        {
            "charge.success" => await HandleChargeSuccess(data, sender, cancellationToken),
            "charge.failed" => await HandleChargeFailed(data, cancellationToken),
            "transfer.success" => await HandleTransferSuccess(data, cancellationToken),
            "transfer.failed" => await HandleTransferFailed(data, cancellationToken),
            _ => Result.Success()
        };
    }

    private async Task<Result> HandleChargeSuccess(
        JsonElement data, ISender sender, CancellationToken ct)
    {
        var reference = data.GetProperty("reference").GetString() ?? string.Empty;
        var amountInSmallestUnit = data.GetProperty("amount").GetInt64();
        var amount = amountInSmallestUnit / 100m;

        // Wallet top-up flow
        if (reference.StartsWith("topup_"))
        {
            var parts = reference.Split('_');
            if (parts.Length >= 2 && Guid.TryParse(parts[1], out var userId))
            {
                var wallet = await walletRepository.GetByUserIdAsync(userId, ct);
                if (wallet is not null)
                    await sender.Send(new CreditWalletCommand(userId, amount, "Wallet top-up via Paystack", reference), ct);
            }
            return Result.Success();
        }

        var record = await repository.GetByReferenceAsync(reference, ct);
        if (record is null || record.Status != PaymentStatus.Pending)
            return Result.Success();

        record.Complete(reference);
        repository.Update(record);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> HandleChargeFailed(JsonElement data, CancellationToken ct)
    {
        var reference = data.GetProperty("reference").GetString() ?? string.Empty;
        var reason = data.TryGetProperty("gateway_response", out var gr) ? gr.GetString() : null;

        var record = await repository.GetByReferenceAsync(reference, ct);
        if (record is null || record.Status != PaymentStatus.Pending)
            return Result.Success();

        record.Fail(reason);
        repository.Update(record);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> HandleTransferSuccess(JsonElement data, CancellationToken ct)
    {
        var reference = data.TryGetProperty("reference", out var r) ? r.GetString() ?? string.Empty : string.Empty;

        var payout = await coachPayoutRepository.GetPayoutByTransferReferenceAsync(reference, ct);
        if (payout is null)
            return Result.Success();

        payout.MarkSuccess();
        coachPayoutRepository.UpdatePayout(payout);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> HandleTransferFailed(JsonElement data, CancellationToken ct)
    {
        var reference = data.TryGetProperty("reference", out var r) ? r.GetString() ?? string.Empty : string.Empty;
        var reason = data.TryGetProperty("reason", out var rs) ? rs.GetString() : "Transfer failed";

        var payout = await coachPayoutRepository.GetPayoutByTransferReferenceAsync(reference, ct);
        if (payout is null)
            return Result.Success();

        payout.MarkFailed(reason ?? "Transfer failed");
        coachPayoutRepository.UpdatePayout(payout);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
