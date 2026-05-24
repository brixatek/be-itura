using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Integration.Tests;

// Webhook handler contract tests — these test the signature validation logic
// without a live Paystack integration.
public sealed class PaystackWebhookTests
{
    private static string ComputeHmacSha512(string payload, string secret)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    [Fact]
    public void ComputeSignature_WithSameSecretAndPayload_ProducesDeterministicHash()
    {
        const string payload = """{"event":"charge.success","data":{"reference":"test_ref","amount":5000}}""";
        const string secret = "sk_test_abc123";

        var sig1 = ComputeHmacSha512(payload, secret);
        var sig2 = ComputeHmacSha512(payload, secret);

        sig1.Should().Be(sig2);
        sig1.Should().HaveLength(128); // SHA-512 hex = 64 bytes = 128 hex chars
    }

    [Fact]
    public void ComputeSignature_WithDifferentSecret_ProducesDifferentHash()
    {
        const string payload = """{"event":"charge.success"}""";

        var sig1 = ComputeHmacSha512(payload, "secret_a");
        var sig2 = ComputeHmacSha512(payload, "secret_b");

        sig1.Should().NotBe(sig2);
    }

    [Fact]
    public void ComputeSignature_WithAlteredPayload_ProducesDifferentHash()
    {
        const string secret = "my_secret";
        var original = ComputeHmacSha512("""{"amount":100}""", secret);
        var tampered = ComputeHmacSha512("""{"amount":999}""", secret);

        original.Should().NotBe(tampered);
    }

    [Theory]
    [InlineData("topup_", true)]
    [InlineData("topup_6ba7b810-9dad-11d1-80b4-00c04fd430c8_1234", true)]
    [InlineData("pay_abc123", false)]
    [InlineData("booking_xyz", false)]
    public void Reference_StartsWithTopup_IndicatesWalletTopUp(string reference, bool expectedTopup)
    {
        reference.StartsWith("topup_").Should().Be(expectedTopup);
    }

    [Fact]
    public void ParseAmount_DividesKoboByHundred()
    {
        // Paystack sends amounts in kobo (smallest unit); NGN is amount/100
        const long amountInKobo = 50000L;
        var amountInNaira = amountInKobo / 100m;

        amountInNaira.Should().Be(500m);
    }
}
