using Itura.Auth.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Auth.Infrastructure.Services;

public sealed class TotpService : ITotpService
{
    private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    private readonly byte[] _encryptionKey;

    public TotpService(IConfiguration config)
    {
        var keyBase64 = config["Mfa:EncryptionKey"] ?? string.Empty;
        _encryptionKey = string.IsNullOrEmpty(keyBase64)
            ? new byte[32]
            : Convert.FromBase64String(keyBase64);
    }

    public byte[] GenerateSecret()
    {
        var secret = new byte[20];
        RandomNumberGenerator.Fill(secret);
        return secret;
    }

    public string ToBase32(byte[] input)
    {
        var sb = new StringBuilder();
        int buffer = input[0];
        int bitsLeft = 8;
        int pos = 0;

        while (bitsLeft > 0 || pos < input.Length - 1)
        {
            if (bitsLeft < 5)
            {
                if (pos < input.Length - 1)
                {
                    pos++;
                    buffer = (buffer << 8) | (input[pos] & 0xFF);
                    bitsLeft += 8;
                }
                else break;
            }
            bitsLeft -= 5;
            sb.Append(Base32Chars[(buffer >> bitsLeft) & 0x1F]);
        }
        return sb.ToString();
    }

    public byte[] FromBase32(string base32)
    {
        base32 = base32.ToUpperInvariant().TrimEnd('=');
        var output = new byte[base32.Length * 5 / 8];
        int buffer = 0, bitsLeft = 0, index = 0;

        foreach (var c in base32)
        {
            var val = Base32Chars.IndexOf(c);
            if (val < 0) continue;
            buffer = (buffer << 5) | val;
            bitsLeft += 5;
            if (bitsLeft >= 8)
            {
                bitsLeft -= 8;
                if (index < output.Length)
                    output[index++] = (byte)(buffer >> bitsLeft);
            }
        }
        return output[..index];
    }

    public string Generate(byte[] secret) => ComputeTotp(secret, 0);

    public bool Verify(byte[] secret, string code)
    {
        for (int window = -1; window <= 1; window++)
            if (ComputeTotp(secret, window) == code) return true;
        return false;
    }

    public string BuildUri(string email, string base32Secret, string issuer = "Itura")
    {
        var label = Uri.EscapeDataString($"{issuer}:{email}");
        var iss = Uri.EscapeDataString(issuer);
        return $"otpauth://totp/{label}?secret={base32Secret}&issuer={iss}&algorithm=SHA1&digits=6&period=30";
    }

    public string EncryptSecret(byte[] secret)
    {
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        var cipher = new byte[secret.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_encryptionKey, 16);
        aes.Encrypt(nonce, secret, cipher, tag);

        var combined = new byte[nonce.Length + tag.Length + cipher.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipher, 0, combined, nonce.Length + tag.Length, cipher.Length);
        return Convert.ToBase64String(combined);
    }

    public byte[] DecryptSecret(string encryptedBase64)
    {
        var combined = Convert.FromBase64String(encryptedBase64);
        var nonce = combined[..12];
        var tag = combined[12..28];
        var cipher = combined[28..];
        var plain = new byte[cipher.Length];

        using var aes = new AesGcm(_encryptionKey, 16);
        aes.Decrypt(nonce, cipher, tag, plain);
        return plain;
    }

    public string HashBackupCode(string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code))).ToLowerInvariant();

    public List<string> GenerateBackupCodes()
    {
        var codes = new List<string>(8);
        for (int i = 0; i < 8; i++)
        {
            var bytes = new byte[5];
            RandomNumberGenerator.Fill(bytes);
            codes.Add(Convert.ToHexString(bytes).ToLowerInvariant());
        }
        return codes;
    }

    private static string ComputeTotp(byte[] secret, int windowOffset)
    {
        long counter = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30) + windowOffset;
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian) Array.Reverse(counterBytes);

        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(counterBytes);

        int offset = hash[^1] & 0x0F;
        int code = ((hash[offset] & 0x7F) << 24)
                 | ((hash[offset + 1] & 0xFF) << 16)
                 | ((hash[offset + 2] & 0xFF) << 8)
                 | (hash[offset + 3] & 0xFF);

        return (code % 1_000_000).ToString("D6");
    }
}
