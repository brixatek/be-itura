using FluentValidation;

namespace Itura.Booking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.CoachId).NotEmpty().WithMessage("Coach ID is required.");
        RuleFor(x => x.CoachUserId).NotEmpty().WithMessage("Coach user ID is required.");
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow).WithMessage("Booking must be scheduled in the future.");
        RuleFor(x => x.DurationMinutes).InclusiveBetween(15, 480).WithMessage("Duration must be between 15 and 480 minutes.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
        RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter code.");
    }
}
