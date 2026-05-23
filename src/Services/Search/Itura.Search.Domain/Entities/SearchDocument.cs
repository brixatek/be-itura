using Itura.SharedKernel.Domain;

namespace Itura.Search.Domain.Entities;

public sealed class SearchDocument : AggregateRoot
{
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string[] Tags { get; private set; } = [];
    public bool IsActive { get; private set; }

    private SearchDocument() { }

    public static SearchDocument Create(string entityType, Guid entityId, string title, string? description, string[] tags)
    {
        return new SearchDocument
        {
            EntityType = entityType,
            EntityId = entityId,
            Title = title,
            Description = description,
            Tags = tags,
            IsActive = true
        };
    }

    public void Update(string title, string? description, string[] tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}
