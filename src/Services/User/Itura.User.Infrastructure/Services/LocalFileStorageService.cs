using Itura.User.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Itura.User.Infrastructure.Services;

public sealed class LocalFileStorageService(
    IWebHostEnvironment env,
    IConfiguration config,
    ILogger<LocalFileStorageService> logger) : IFileStorageService
{
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        if (!AllowedTypes.Contains(contentType.ToLowerInvariant()))
            throw new InvalidOperationException($"Content type '{contentType}' is not allowed.");

        var ext = contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => Path.GetExtension(fileName)
        };

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var uploadsDir = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, uniqueName);
        await using var file = File.Create(filePath);
        await stream.CopyToAsync(file, ct);

        var baseUrl = config["App:BaseUrl"] ?? "http://localhost:5000";
        var url = $"{baseUrl.TrimEnd('/')}/uploads/avatars/{uniqueName}";
        logger.LogInformation("Uploaded avatar to {Path}", filePath);
        return url;
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            var fileName = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
            var uploadsDir = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "uploads", "avatars");
            var path = Path.Combine(uploadsDir, fileName);
            if (File.Exists(path)) File.Delete(path);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete avatar file {Url}", fileUrl);
        }
        return Task.CompletedTask;
    }
}
