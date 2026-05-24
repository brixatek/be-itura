using Itura.Contracts.Booking;
using Itura.Contracts.Payment;
using MassTransit;

namespace Itura.Booking.Infrastructure.Sagas;

public sealed class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public State AwaitingPayment { get; private set; } = null!;
    public State Created { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    public Event<BookingCreatedEvent> BookingCreated { get; private set; } = null!;
    public Event<BookingConfirmedEvent> BookingConfirmed { get; private set; } = null!;
    public Event<BookingCompletedEvent> BookingCompleted { get; private set; } = null!;
    public Event<BookingCancelledEvent> BookingCancelled { get; private set; } = null!;
    public Event<PaymentCompletedEvent> PaymentCompleted { get; private set; } = null!;
    public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;

    public BookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingCreated, x => x.CorrelateById(ctx => ctx.Message.BookingId));
        Event(() => BookingConfirmed, x => x.CorrelateById(ctx => ctx.Message.BookingId));
        Event(() => BookingCompleted, x => x.CorrelateById(ctx => ctx.Message.BookingId));
        Event(() => BookingCancelled, x => x.CorrelateById(ctx => ctx.Message.BookingId));
        Event(() => PaymentCompleted, x => x.CorrelateById(ctx => ctx.Message.BookingId));
        Event(() => PaymentFailed, x => x.CorrelateById(ctx => ctx.Message.BookingId));

        // Booking created → wait for client to complete payment before coach can confirm
        Initially(
            When(BookingCreated)
                .Then(ctx =>
                {
                    ctx.Saga.CoachId = ctx.Message.CoachId;
                    ctx.Saga.CoachUserId = ctx.Message.CoachUserId;
                    ctx.Saga.ClientUserId = ctx.Message.ClientUserId;
                    ctx.Saga.ScheduledAt = ctx.Message.ScheduledAt;
                    ctx.Saga.Price = ctx.Message.Price;
                    ctx.Saga.Currency = ctx.Message.Currency;
                    ctx.Saga.CreatedAt = ctx.Message.CreatedAt;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(AwaitingPayment));

        // Payment confirmed → booking moves to Created (awaiting coach confirmation)
        // Payment failed → booking cancelled automatically
        During(AwaitingPayment,
            When(PaymentCompleted)
                .Then(ctx => ctx.Saga.UpdatedAt = DateTime.UtcNow)
                .TransitionTo(Created),
            When(PaymentFailed)
                .Then(ctx =>
                {
                    ctx.Saga.CancellationReason = ctx.Message.FailureReason ?? "Payment failed.";
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Cancelled),
            When(BookingCancelled)
                .Then(ctx =>
                {
                    ctx.Saga.CancellationReason = ctx.Message.CancellationReason;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Cancelled));

        // Coach confirms or cancels after payment is received
        During(Created,
            When(BookingConfirmed)
                .Then(ctx =>
                {
                    ctx.Saga.ScheduledAt = ctx.Message.ScheduledAt;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Confirmed),
            When(BookingCancelled)
                .Then(ctx =>
                {
                    ctx.Saga.CancellationReason = ctx.Message.CancellationReason;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Cancelled));

        During(Confirmed,
            When(BookingCompleted)
                .Then(ctx => ctx.Saga.UpdatedAt = DateTime.UtcNow)
                .TransitionTo(Completed),
            When(BookingCancelled)
                .Then(ctx =>
                {
                    ctx.Saga.CancellationReason = ctx.Message.CancellationReason;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Cancelled));

        // Allow rescheduling: client pays again and booking re-enters AwaitingPayment
        During(Cancelled,
            When(BookingCreated)
                .Then(ctx =>
                {
                    ctx.Saga.ScheduledAt = ctx.Message.ScheduledAt;
                    ctx.Saga.CancellationReason = null;
                    ctx.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(AwaitingPayment));

        SetCompletedWhenFinalized();
    }
}
