using FluentValidation;

namespace Itura.Payment.Application.Features.Payments.InitiatePayment;

public sealed class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage("Booking ID is required.");
        RuleFor(x => x.PayeeUserId).NotEmpty().WithMessage("Payee user ID is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter code.");
        RuleFor(x => x.PaymentMethod).MaximumLength(50).When(x => x.PaymentMethod is not null);
    }
}
