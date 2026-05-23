using Itura.Payment.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Itura.Payment.Infrastructure.Services;

internal sealed class FieldEncryptionService(IOptions<PaystackOptions> options) : IFieldEncryptionService
{
    private readonly byte[] _key = Convert.FromBase64String(
        options.Value.EncryptionKey ?? throw new InvalidOperationException("FieldEncryption key not configured."));

    public string Encrypt(string plaintext)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        var tag = new byte[16];
        var ciphertext = new byte[plaintextBytes.Length];

        using var aes = new AesGcm(_key, 16);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, nonce.Length);
        ciphertext.CopyTo(result, nonce.Length + tag.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string ciphertext)
    {
        try
        {
            var data = Convert.FromBase64String(ciphertext);
            if (data.Length < 28) return ciphertext;

            var nonce = data[..12];
            var tag = data[12..28];
            var encrypted = data[28..];
            var plaintext = new byte[encrypted.Length];

            using var aes = new AesGcm(_key, 16);
            aes.Decrypt(nonce, encrypted, tag, plaintext);
            return Encoding.UTF8.GetString(plaintext);
        }
        catch
        {
            return ciphertext;
        }
    }
}
