namespace Itura.Media.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> StoreAsync(byte[] content, string fileName, string mimeType, CancellationToken ct = default);
    Task DeleteAsync(string storedFileName, CancellationToken ct = default);
    string GetUrl(string storedFileName);
}
