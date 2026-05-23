namespace Itura.Journal.Infrastructure.Services;

public sealed class EncryptionOptions
{
    public const string Section = "Encryption";
    public string Key { get; init; } = string.Empty;
}
