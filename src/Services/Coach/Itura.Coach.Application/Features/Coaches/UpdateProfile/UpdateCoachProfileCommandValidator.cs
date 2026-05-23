using FluentValidation;

namespace Itura.Coach.Application.Features.Coaches.UpdateProfile;

public sealed class UpdateCoachProfileCommandValidator : AbstractValidator<UpdateCoachProfileCommand>
{
    public UpdateCoachProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters.");

        RuleFor(x => x.Specializations)
            .Must(s => s.Count <= 20).WithMessage("Cannot have more than 20 specializations.");

        RuleFor(x => x.Languages)
            .NotEmpty().WithMessage("At least one language is required.")
            .Must(l => l.Count <= 10).WithMessage("Cannot have more than 10 languages.");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hourly rate cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter code (e.g. USD).");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(60).WithMessage("Years of experience seems too high.");
    }
}
