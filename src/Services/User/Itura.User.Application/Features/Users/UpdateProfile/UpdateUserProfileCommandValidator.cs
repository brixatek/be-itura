using FluentValidation;

namespace Itura.User.Application.Features.Users.UpdateProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required.")
            .MaximumLength(50).WithMessage("Timezone must not exceed 50 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(x => x.Bio is not null);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("Avatar URL must not exceed 500 characters.")
            .When(x => x.AvatarUrl is not null);
    }
}
