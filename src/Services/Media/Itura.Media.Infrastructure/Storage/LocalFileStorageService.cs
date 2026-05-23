using Itura.Media.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Itura.Media.Infrastructure.Storage;

internal sealed class LocalFileStorageService(IConfiguration config) : IFileStorageService
{
    private string StoragePath => config["FileStorage:LocalPath"] ?? Path.Combine(Path.GetTempPath(), "itura_media");
    private string BaseUrl => config["FileStorage:BaseUrl"] ?? "http://localhost:5098/media/files";

    public async Task<string> StoreAsync(byte[] content, string fileName, string mimeType, CancellationToken ct = default)
    {
        Directory.CreateDirectory(StoragePath);

        var ext = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(StoragePath, storedFileName);

        await File.WriteAllBytesAsync(fullPath, content, ct);
        return storedFileName;
    }

    public Task DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(StoragePath, storedFileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetUrl(string storedFileName) => $"{BaseUrl.TrimEnd('/')}/{storedFileName}";
}
