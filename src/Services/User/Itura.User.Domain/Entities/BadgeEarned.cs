using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class BadgeEarned : AuditableEntity
{
    public Guid UserProfileId { get; private set; }
    public Guid BadgeDefinitionId { get; private set; }
    public BadgeDefinition? BadgeDefinition { get; private set; }

    private BadgeEarned() { }

    public static BadgeEarned Create(Guid userProfileId, Guid badgeDefinitionId) =>
        new()
        {
            UserProfileId = userProfileId,
            BadgeDefinitionId = badgeDefinitionId
        };
}
