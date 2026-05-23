using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class XpTransaction : AuditableEntity
{
    public Guid UserProfileId { get; private set; }
    public int Amount { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public Guid? ReferenceId { get; private set; }

    private XpTransaction() { }

    public static XpTransaction Create(Guid userProfileId, int amount, string action, Guid? referenceId = null) =>
        new()
        {
            UserProfileId = userProfileId,
            Amount = amount,
            Action = action,
            ReferenceId = referenceId
        };
}
