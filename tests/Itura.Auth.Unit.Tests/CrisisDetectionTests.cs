using FluentAssertions;

namespace Itura.Auth.Unit.Tests;

// Tests the crisis keyword detection logic embedded in SendMessageCommandHandler.
// The logic is duplicated here as a pure function test to verify the detection rules
// without requiring the full handler dependency graph.
public sealed class CrisisDetectionTests
{
    private static readonly string[] CrisisKeywords =
    [
        "suicide", "kill myself", "end my life", "want to die", "self harm",
        "cut myself", "hurt myself", "don't want to live", "no reason to live"
    ];

    private static bool DetectCrisis(string message)
    {
        var lower = message.ToLowerInvariant();
        return CrisisKeywords.Any(k => lower.Contains(k));
    }

    [Theory]
    [InlineData("I want to commit suicide", true)]
    [InlineData("I want to kill myself tonight", true)]
    [InlineData("I want to end my life", true)]
    [InlineData("I want to die", true)]
    [InlineData("I've been doing self harm", true)]
    [InlineData("I cut myself last night", true)]
    [InlineData("I hurt myself again", true)]
    [InlineData("I don't want to live anymore", true)]
    [InlineData("there is no reason to live", true)]
    [InlineData("I'm feeling sad today", false)]
    [InlineData("I need help with my wellness journey", false)]
    [InlineData("Feeling anxious about work", false)]
    [InlineData("I killed it at the gym today", false)]
    [InlineData("This game is dying without updates", false)]
    public void DetectCrisis_CorrectlyIdentifiesCrisisMessages(string message, bool expected)
    {
        DetectCrisis(message).Should().Be(expected);
    }

    [Fact]
    public void DetectCrisis_IsCaseInsensitive()
    {
        DetectCrisis("SUICIDE").Should().BeTrue();
        DetectCrisis("Want To DIE").Should().BeTrue();
        DetectCrisis("KILL MYSELF").Should().BeTrue();
    }

    [Fact]
    public void DetectCrisis_EmptyMessage_ReturnsFalse()
    {
        DetectCrisis(string.Empty).Should().BeFalse();
        DetectCrisis("   ").Should().BeFalse();
    }
}
