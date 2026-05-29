namespace Itura.User.Application.Common.Interfaces;

public interface IImageResizingService
{
    /// <summary>
    /// Returns a resized copy of the image stream at the specified dimensions.
    /// Output is always JPEG for consistency.
    /// </summary>
    Task<Stream> ResizeAsync(Stream source, int width, int height, CancellationToken ct = default);
}
