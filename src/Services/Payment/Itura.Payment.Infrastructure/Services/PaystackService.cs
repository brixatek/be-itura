using Itura.Payment.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Itura.Payment.Infrastructure.Services;

internal sealed class PaystackService(
    IHttpClientFactory httpClientFactory,
    IOptions<PaystackOptions> options,
    ILogger<PaystackService> logger)
    : IPaystackService
{
    private readonly PaystackOptions _opts = options.Value;

    public async Task<PaystackInitResult> InitializeAsync(
        string email, long amountInSmallestUnit, string reference,
        string? callbackUrl = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.SecretKey))
        {
            logger.LogWarning("[DEV] Paystack not configured — returning mock authorization URL for reference {Reference}", reference);
            return new PaystackInitResult(true, $"https://paystack.com/pay/mock_{reference}", reference, reference, null);
        }

        try
        {
            var client = httpClientFactory.CreateClient("Paystack");
            var body = new
            {
                email,
                amount = amountInSmallestUnit,
                reference,
                callback_url = callbackUrl ?? _opts.CallbackUrl
            };

            var response = await client.PostAsJsonAsync("/transaction/initialize", body, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<PaystackApiResponse<PaystackInitData>>(cancellationToken: ct);
            if (json?.Status != true || json.Data is null)
                return new PaystackInitResult(false, null, null, null, json?.Message);

            return new PaystackInitResult(true, json.Data.AuthorizationUrl, json.Data.AccessCode, json.Data.Reference, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Paystack initialization failed for reference {Reference}", reference);
            return new PaystackInitResult(false, null, null, null, ex.Message);
        }
    }

    public async Task<PaystackVerifyResult> VerifyAsync(string reference, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.SecretKey))
        {
            logger.LogWarning("[DEV] Paystack not configured — returning mock success for reference {Reference}", reference);
            return new PaystackVerifyResult(true, "success", 0, reference, null);
        }

        try
        {
            var client = httpClientFactory.CreateClient("Paystack");
            var json = await client.GetFromJsonAsync<PaystackApiResponse<PaystackTransactionData>>(
                $"/transaction/verify/{Uri.EscapeDataString(reference)}", ct);

            if (json?.Status != true || json.Data is null)
                return new PaystackVerifyResult(false, "unknown", 0, reference, json?.Message);

            return new PaystackVerifyResult(true, json.Data.Status, json.Data.Amount, json.Data.Reference, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Paystack verification failed for reference {Reference}", reference);
            return new PaystackVerifyResult(false, "error", 0, reference, ex.Message);
        }
    }

    public bool ValidateWebhookSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_opts.SecretKey))
            return true; // Dev mode — skip validation

        var key = Encoding.UTF8.GetBytes(_opts.SecretKey);
        var data = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA512(key);
        var hash = hmac.ComputeHash(data);
        var computed = Convert.ToHexString(hash).ToLowerInvariant();
        return string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<PaystackTransferResult> CreateTransferRecipientAsync(
        string bankCode, string accountNumber, string accountName, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.SecretKey))
        {
            logger.LogWarning("[DEV] Paystack not configured — mock transfer recipient created");
            return new PaystackTransferResult(true, $"RCP_mock_{accountNumber}", null, null);
        }

        try
        {
            var client = httpClientFactory.CreateClient("Paystack");
            var body = new { type = "nuban", name = accountName, account_number = accountNumber, bank_code = bankCode, currency = "NGN" };
            var response = await client.PostAsJsonAsync("/transferrecipient", body, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<PaystackApiResponse<PaystackRecipientData>>(cancellationToken: ct);
            if (json?.Status != true || json.Data is null)
                return new PaystackTransferResult(false, null, null, json?.Message);
            return new PaystackTransferResult(true, json.Data.RecipientCode, null, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Paystack transfer recipient");
            return new PaystackTransferResult(false, null, null, ex.Message);
        }
    }

    public async Task<PaystackTransferResult> InitiateTransferAsync(
        string recipientCode, long amountInSmallestUnit, string reference, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_opts.SecretKey))
        {
            logger.LogWarning("[DEV] Paystack not configured — mock transfer initiated for reference {Reference}", reference);
            return new PaystackTransferResult(true, null, reference, null);
        }

        try
        {
            var client = httpClientFactory.CreateClient("Paystack");
            var body = new { source = "balance", amount = amountInSmallestUnit, recipient = recipientCode, reference, reason = "Coach payout" };
            var response = await client.PostAsJsonAsync("/transfer", body, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<PaystackApiResponse<PaystackTransferData>>(cancellationToken: ct);
            if (json?.Status != true || json.Data is null)
                return new PaystackTransferResult(false, null, null, json?.Message);
            return new PaystackTransferResult(true, json.Data.TransferCode, json.Data.Reference, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initiate Paystack transfer for reference {Reference}", reference);
            return new PaystackTransferResult(false, null, null, ex.Message);
        }
    }

    // ── Internal Paystack API response shapes ────────────────────────────────

    private sealed record PaystackApiResponse<T>(
        [property: JsonPropertyName("status")] bool Status,
        [property: JsonPropertyName("message")] string? Message,
        [property: JsonPropertyName("data")] T? Data);

    private sealed record PaystackInitData(
        [property: JsonPropertyName("authorization_url")] string AuthorizationUrl,
        [property: JsonPropertyName("access_code")] string AccessCode,
        [property: JsonPropertyName("reference")] string Reference);

    private sealed record PaystackTransactionData(
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("amount")] long Amount,
        [property: JsonPropertyName("reference")] string Reference);

    private sealed record PaystackRecipientData(
        [property: JsonPropertyName("recipient_code")] string RecipientCode);

    private sealed record PaystackTransferData(
        [property: JsonPropertyName("transfer_code")] string TransferCode,
        [property: JsonPropertyName("reference")] string Reference);
}
