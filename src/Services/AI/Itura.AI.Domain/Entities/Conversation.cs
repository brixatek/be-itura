namespace Itura.AI.Domain.Entities;

public sealed class Conversation
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public Guid UserId { get; init; }
    public string Title { get; set; } = "New Conversation";
    public List<ConversationMessage> Messages { get; init; } = [];
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void AddMessage(string role, string content)
    {
        Messages.Add(new ConversationMessage(role, content, DateTime.UtcNow));
        UpdatedAt = DateTime.UtcNow;
        if (role == "user" && Messages.Count == 1)
            Title = content.Length > 50 ? content[..50] + "…" : content;
    }
}

public sealed record ConversationMessage(string Role, string Content, DateTime Timestamp);
