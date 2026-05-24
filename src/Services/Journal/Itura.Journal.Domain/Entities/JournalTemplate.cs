using Itura.SharedKernel.Entities;

namespace Itura.Journal.Domain.Entities;

public sealed class JournalTemplate : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Prompt { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;

    private JournalTemplate() { }

    public static JournalTemplate Create(Guid id, string name, string description, string prompt, string category) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            Prompt = prompt,
            Category = category
        };
}
