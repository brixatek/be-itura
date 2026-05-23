using FluentValidation;

namespace Itura.Community.Application.Features.Posts.Commands.CreatePost;

internal sealed class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    private static readonly string[] ValidTypes = ["General", "Question", "Tip", "Milestone"];

    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.PostType).NotEmpty().Must(t => ValidTypes.Contains(t))
            .WithMessage("PostType must be one of: General, Question, Tip, Milestone.");
        RuleFor(x => x.AuthorUserId).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(300).When(x => x.Title != null);
    }
}
