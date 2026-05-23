using FluentValidation;

namespace Itura.Content.Application.Features.Content.Commands.CreateContent;

internal sealed class CreateContentCommandValidator : AbstractValidator<CreateContentCommand>
{
    private static readonly string[] ValidTypes = ["Article", "Video", "Audio", "Podcast"];

    public CreateContentCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ContentType).NotEmpty().Must(t => ValidTypes.Contains(t))
            .WithMessage("ContentType must be one of: Article, Video, Audio, Podcast.");
        RuleFor(x => x.AuthorUserId).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description != null);
        RuleFor(x => x.DurationSeconds).GreaterThan(0).When(x => x.DurationSeconds.HasValue);
    }
}
