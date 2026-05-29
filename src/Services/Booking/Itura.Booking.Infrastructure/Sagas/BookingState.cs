using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace Itura.Booking.Infrastructure.Sagas;

public sealed class BookingState : SagaStateMachineInstance
{
    [Key]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid CoachId { get; set; }
    public Guid CoachUserId { get; set; }
    public Guid ClientUserId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = null!;
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
