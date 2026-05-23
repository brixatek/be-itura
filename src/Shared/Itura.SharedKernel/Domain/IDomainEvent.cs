using MediatR;

namespace Itura.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
