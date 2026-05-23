using FluentValidation;

namespace Itura.Booking.Application.Features.Bookings.CancelBooking;

public sealed class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(500).When(x => x.Reason is not null)
            .WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
