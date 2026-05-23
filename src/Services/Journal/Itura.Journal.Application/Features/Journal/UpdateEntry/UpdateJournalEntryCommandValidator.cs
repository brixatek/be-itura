using FluentValidation;

namespace Itura.Journal.Application.Features.Journal.UpdateEntry;

public sealed class UpdateJournalEntryCommandValidator : AbstractValidator<UpdateJournalEntryCommand>
{
    public UpdateJournalEntryCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Content)
            .MaximumLength(50_000).WithMessage("Content must not exceed 50,000 characters.");

        RuleFor(x => x.Tags)
            .Must(t => t.Count <= 20).WithMessage("Cannot have more than 20 tags.");

        RuleFor(x => x.MoodScore)
            .InclusiveBetween(1, 10).When(x => x.MoodScore.HasValue)
            .WithMessage("Mood score must be between 1 and 10.");
    }
}
