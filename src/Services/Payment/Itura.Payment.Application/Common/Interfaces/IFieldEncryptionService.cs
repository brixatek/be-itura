namespace Itura.Payment.Application.Common.Interfaces;

public interface IFieldEncryptionService
{
    string Encrypt(string plaintext);
    string Decrypt(string ciphertext);
}
