using Itura.Booking.Domain.Enums;
using Itura.Booking.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Booking.Domain.Entities;

public sealed class BookingSession : AggregateRoot
{
    public Guid CoachId { get; private set; }
    public Guid CoachUserId { get; private set; }
    public Guid ClientUserId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public BookingStatus Status { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? CancellationReason { get; private set; }

    private BookingSession() { }

    public static Result<BookingSession> Create(
        Guid coachId, Guid coachUserId, Guid clientUserId,
        DateTime scheduledAt, int durationMinutes,
        decimal price, string currency)
    {
        if (coachUserId == clientUserId)
            return Result.Failure<BookingSession>(Error.Validation("Booking.SelfBooking", "You cannot book yourself."));
        if (scheduledAt.ToUniversalTime() <= DateTime.UtcNow)
            return Result.Failure<BookingSession>(Error.Validation("Booking.ScheduledAt", "Booking must be scheduled in the future."));
        if (durationMinutes is < 15 or > 480)
            return Result.Failure<BookingSession>(Error.Validation("Booking.DurationMinutes", "Duration must be between 15 and 480 minutes."));
        if (price < 0)
            return Result.Failure<BookingSession>(Error.Validation("Booking.Price", "Price cannot be negative."));

        var normalizedCurrency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();

        var session = new BookingSession
        {
            Id = Guid.NewGuid(),
            CoachId = coachId,
            CoachUserId = coachUserId,
            ClientUserId = clientUserId,
            ScheduledAt = scheduledAt.ToUniversalTime(),
            DurationMinutes = durationMinutes,
            Status = BookingStatus.Pending,
            Price = price,
            Currency = normalizedCurrency
        };

        session.RaiseDomainEvent(new BookingCreatedDomainEvent(
            session.Id, coachId, coachUserId, clientUserId,
            session.ScheduledAt, durationMinutes, price, normalizedCurrency));

        return Result.Success(session);
    }

    public Result Confirm()
    {
        if (Status != BookingStatus.Pending)
            return Result.Failure(Error.Validation("Booking.Confirm", "Only pending bookings can be confirmed."));

        Status = BookingStatus.Confirmed;
        MarkUpdated();
        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id, CoachId, ClientUserId, ScheduledAt, DurationMinutes, Price, Currency));
        return Result.Success();
    }

    public Result Cancel(string? reason = null)
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed)
            return Result.Failure(Error.Validation("Booking.Cancel", "Cannot cancel a booking that is already cancelled or completed."));

        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        MarkUpdated();
        RaiseDomainEvent(new BookingCancelledDomainEvent(Id, CoachId, ClientUserId, reason));
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(Error.Validation("Booking.Complete", "Only confirmed bookings can be completed."));

        Status = BookingStatus.Completed;
        MarkUpdated();
        RaiseDomainEvent(new BookingCompletedDomainEvent(Id, CoachId, ClientUserId, Price, Currency));
        return Result.Success();
    }
}
