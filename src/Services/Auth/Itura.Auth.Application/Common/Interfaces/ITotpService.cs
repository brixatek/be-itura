namespace Itura.Auth.Application.Common.Interfaces;

public interface ITotpService
{
    /// <summary>Generates a random Base32 TOTP secret and returns it unencoded.</summary>
    byte[] GenerateSecret();

    /// <summary>Encodes secret bytes to Base32 string for display / QR code.</summary>
    string ToBase32(byte[] secret);

    /// <summary>Decodes a Base32 string back to bytes.</summary>
    byte[] FromBase32(string base32);

    /// <summary>Generates the current TOTP code for the given secret bytes.</summary>
    string Generate(byte[] secret);

    /// <summary>Validates a TOTP code, allowing a window of ±1 time-step.</summary>
    bool Verify(byte[] secret, string code);

    /// <summary>Returns the otpauth:// URI for QR code generation.</summary>
    string BuildUri(string email, string base32Secret, string issuer = "Itura");

    /// <summary>Encrypts secret bytes to Base64 string using AES-256-GCM.</summary>
    string EncryptSecret(byte[] secret);

    /// <summary>Decrypts a Base64-encoded encrypted secret back to bytes.</summary>
    byte[] DecryptSecret(string encryptedBase64);

    /// <summary>Hashes a backup code using SHA-256 for safe storage.</summary>
    string HashBackupCode(string code);

    /// <summary>Generates 8 random backup codes.</summary>
    List<string> GenerateBackupCodes();
}
