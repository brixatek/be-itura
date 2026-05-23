using Itura.Media.Domain.Enums;
using Itura.SharedKernel.Domain;

namespace Itura.Media.Domain.Events;

public sealed record MediaAssetUploadedDomainEvent(
    Guid MediaAssetId,
    string Url,
    MediaType MediaType,
    string MimeType,
    Guid UploaderUserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
