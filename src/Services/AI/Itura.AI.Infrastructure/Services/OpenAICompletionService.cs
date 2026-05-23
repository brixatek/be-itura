using Itura.AI.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Itura.AI.Infrastructure.Services;

internal sealed class OpenAICompletionService(
    IHttpClientFactory httpClientFactory,
    IOptions<OpenAIOptions> options,
    ILogger<OpenAICompletionService> logger) : IAICompletionService
{
    private static readonly string[] MockReplies =
    [
        "It sounds like you're going through something meaningful. I'm here to listen — would you like to share more about how that's affecting you?",
        "Thank you for opening up. Your feelings are valid, and it takes courage to reflect on them. What would feel most supportive right now?",
        "That's a thoughtful reflection. Building self-awareness like this is a real strength. What small step could you take today?"
    ];

    private static readonly string[] MockPrompts =
    [
        "What made you feel most alive today, and why?",
        "Describe a recent challenge and what you learned from it.",
        "What are three things you're grateful for right now?"
    ];

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        string systemPrompt,
        IEnumerable<(string Role, string Content)> history,
        string userMessage,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(options.Value.ApiKey))
        {
            logger.LogWarning("OpenAI ApiKey not configured — streaming mock response");
            var reply = MockReplies[Random.Shared.Next(MockReplies.Length)];
            foreach (var word in reply.Split(' '))
            {
                yield return word + " ";
                await Task.Delay(50, ct);
            }
            yield break;
        }

        var messages = BuildMessages(systemPrompt, history, userMessage);
        var requestBody = JsonSerializer.Serialize(new
        {
            model = options.Value.Model,
            messages,
            stream = true,
            max_tokens = 500
        });

        var client = httpClientFactory.CreateClient("OpenAI");
        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrEmpty(line) || !line.StartsWith("data: ")) continue;
            var data = line["data: ".Length..];
            if (data == "[DONE]") break;

            var chunk = JsonDocument.Parse(data);
            var content = chunk.RootElement
                .GetProperty("choices")[0]
                .GetProperty("delta")
                .GetProperty("content")
                .GetString();

            if (!string.IsNullOrEmpty(content))
                yield return content;
        }
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(options.Value.ApiKey))
        {
            logger.LogWarning("OpenAI ApiKey not configured — returning mock response");
            return userMessage.Contains("journal", StringComparison.OrdinalIgnoreCase)
                ? string.Join("\n", MockPrompts.Select((p, i) => $"{i + 1}. {p}"))
                : MockReplies[Random.Shared.Next(MockReplies.Length)];
        }

        var messages = BuildMessages(systemPrompt, [], userMessage);
        var requestBody = JsonSerializer.Serialize(new { model = options.Value.Model, messages, max_tokens = 500 });

        var client = httpClientFactory.CreateClient("OpenAI");
        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
    }

    private static object[] BuildMessages(string systemPrompt, IEnumerable<(string Role, string Content)> history, string userMessage)
    {
        var messages = new List<object> { new { role = "system", content = systemPrompt } };
        messages.AddRange(history.Select(m => (object)new { role = m.Role, content = m.Content }));
        messages.Add(new { role = "user", content = userMessage });
        return [.. messages];
    }
}
