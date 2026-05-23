using Itura.Journal.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Journal.Infrastructure.Services;

public sealed class AesGcmEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesGcmEncryptionService(IOptions<EncryptionOptions> opts)
    {
        _key = Convert.FromBase64String(opts.Value.Key);
        if (_key.Length != 32)
            throw new InvalidOperationException("Journal encryption key must be 32 bytes (AES-256).");
    }

    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return plaintext;

        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_key, 16);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // Format: nonce(12) + tag(16) + ciphertext
        var combined = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, nonce.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(combined);
    }

    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext)) return ciphertext;

        byte[] combined;
        try { combined = Convert.FromBase64String(ciphertext); }
        catch { return ciphertext; } // Already plaintext (migration path)

        if (combined.Length < 28) return ciphertext;

        var nonce = combined[..12];
        var tag = combined[12..28];
        var cipher = combined[28..];

        var plainBytes = new byte[cipher.Length];

        try
        {
            using var aes = new AesGcm(_key, 16);
            aes.Decrypt(nonce, cipher, tag, plainBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch { return ciphertext; } // Fallback for unencrypted legacy entries
    }
}
