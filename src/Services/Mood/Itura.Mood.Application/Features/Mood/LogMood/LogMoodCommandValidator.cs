using FluentValidation;

namespace Itura.Mood.Application.Features.Mood.LogMood;

public sealed class LogMoodCommandValidator : AbstractValidator<LogMoodCommand>
{
    public LogMoodCommandValidator()
    {
        RuleFor(x => x.Score).InclusiveBetween(1, 10);
        RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note is not null);
        RuleFor(x => x.Emotions).Must(e => e.Count <= 10).WithMessage("Maximum 10 emotions per entry.");
        RuleFor(x => x.LoggedAt)
            .Must(d => d == null || d <= DateTime.UtcNow)
            .WithMessage("LoggedAt cannot be in the future.");
    }
}
