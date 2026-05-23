namespace Itura.Payment.Application.Common.Interfaces;

public interface IPaystackService
{
    Task<PaystackInitResult> InitializeAsync(string email, long amountInSmallestUnit, string reference, string? callbackUrl = null, CancellationToken ct = default);
    Task<PaystackVerifyResult> VerifyAsync(string reference, CancellationToken ct = default);
    bool ValidateWebhookSignature(string payload, string signature);
    Task<PaystackTransferResult> CreateTransferRecipientAsync(string bankCode, string accountNumber, string accountName, CancellationToken ct = default);
    Task<PaystackTransferResult> InitiateTransferAsync(string recipientCode, long amountInSmallestUnit, string reference, CancellationToken ct = default);
}

public sealed record PaystackInitResult(bool Success, string? AuthorizationUrl, string? AccessCode, string? Reference, string? Message);
public sealed record PaystackVerifyResult(bool Success, string Status, long AmountInSmallestUnit, string Reference, string? Message);
public sealed record PaystackTransferResult(bool Success, string? TransferCode, string? Reference, string? Message);
