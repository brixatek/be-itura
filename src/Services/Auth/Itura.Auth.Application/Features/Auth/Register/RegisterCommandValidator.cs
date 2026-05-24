using FluentValidation;

namespace Itura.Auth.Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required.")
            .MaximumLength(50)
            .Must(tz => TimeZoneInfo.GetSystemTimeZones().Any(t => t.Id == tz))
            .WithMessage("Timezone must be a valid IANA timezone identifier (e.g. 'Africa/Lagos', 'UTC').")
            .When(x => !string.IsNullOrWhiteSpace(x.Timezone));
    }
}
