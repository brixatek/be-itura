namespace Itura.AI.Infrastructure.Services;

public sealed class OpenAIOptions
{
    public const string Section = "OpenAI";
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-4o";
    public string BaseUrl { get; init; } = "https://api.openai.com/v1/";
}
