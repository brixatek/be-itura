using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class BadgeDefinition : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string IconUrl { get; private set; } = string.Empty;
    public string Trigger { get; private set; } = string.Empty;
    public string Condition { get; private set; } = string.Empty;

    private BadgeDefinition() { }

    public static BadgeDefinition Create(Guid id, string name, string description, string iconUrl, string trigger, string condition) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            IconUrl = iconUrl,
            Trigger = trigger,
            Condition = condition
        };
}
