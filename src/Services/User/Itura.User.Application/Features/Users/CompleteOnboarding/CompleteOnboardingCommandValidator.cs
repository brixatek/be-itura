using FluentValidation;

namespace Itura.User.Application.Features.Users.CompleteOnboarding;

public sealed class CompleteOnboardingCommandValidator : AbstractValidator<CompleteOnboardingCommand>
{
    public CompleteOnboardingCommandValidator()
    {
        RuleFor(x => x.WellnessGoals)
            .NotEmpty().WithMessage("At least one wellness goal is required.")
            .Must(g => g.Count <= 10).WithMessage("A maximum of 10 wellness goals are allowed.");

        RuleForEach(x => x.WellnessGoals)
            .NotEmpty().WithMessage("Wellness goal must not be empty.")
            .MaximumLength(100).WithMessage("Each wellness goal must not exceed 100 characters.");
    }
}
