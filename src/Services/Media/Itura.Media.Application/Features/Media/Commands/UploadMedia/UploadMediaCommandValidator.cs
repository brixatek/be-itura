using FluentValidation;

namespace Itura.Media.Application.Features.Media.Commands.UploadMedia;

internal sealed class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    private static readonly string[] ValidTypes = ["Image", "Video", "Audio", "Document"];
    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100 MB

    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.MimeType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MediaType).NotEmpty().Must(t => ValidTypes.Contains(t))
            .WithMessage("MediaType must be one of: Image, Video, Audio, Document.");
        RuleFor(x => x.UploaderUserId).NotEmpty();
        RuleFor(x => x.FileContent).NotEmpty();
        RuleFor(x => x.FileSize).GreaterThan(0).LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("File size must be between 1 byte and 100 MB.");
    }
}
