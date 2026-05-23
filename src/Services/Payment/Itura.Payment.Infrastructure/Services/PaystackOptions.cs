namespace Itura.Payment.Infrastructure.Services;

public sealed class PaystackOptions
{
    public const string Section = "Paystack";
    public string SecretKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://api.paystack.co";
    public string? CallbackUrl { get; init; }
    public string? EncryptionKey { get; init; }
}
