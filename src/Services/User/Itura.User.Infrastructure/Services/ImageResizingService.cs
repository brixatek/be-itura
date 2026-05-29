using Itura.User.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Itura.User.Infrastructure.Services;

internal sealed class ImageResizingService : IImageResizingService
{
    public async Task<Stream> ResizeAsync(Stream source, int width, int height, CancellationToken ct = default)
    {
        using var image = await Image.LoadAsync(source, ct);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop,
            Position = AnchorPositionMode.Center
        }));

        var output = new MemoryStream();
        await image.SaveAsync(output, new JpegEncoder { Quality = 85 }, ct);
        output.Position = 0;
        return output;
    }
}
