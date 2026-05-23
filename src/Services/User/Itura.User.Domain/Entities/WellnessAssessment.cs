using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class WellnessAssessment : AuditableEntity
{
    public Guid UserProfileId { get; private set; }
    public int CompositeScore { get; private set; }
    public string RiskLevel { get; private set; } = "Low";
    public Dictionary<string, int> Answers { get; private set; } = [];

    private WellnessAssessment() { }

    public static WellnessAssessment Create(Guid userProfileId, Dictionary<string, int> answers)
    {
        var score = CalculateScore(answers);
        return new WellnessAssessment
        {
            UserProfileId = userProfileId,
            Answers = answers,
            CompositeScore = score,
            RiskLevel = DetermineRiskLevel(score)
        };
    }

    private static int CalculateScore(Dictionary<string, int> answers)
    {
        if (answers.Count == 0) return 0;
        var total = answers.Values.Sum();
        var max = answers.Count * 10;
        return Math.Min(100, (int)(total * 100.0 / max));
    }

    private static string DetermineRiskLevel(int score) => score switch
    {
        >= 80 => "Low",
        >= 60 => "Moderate",
        >= 40 => "High",
        _ => "Crisis"
    };
}
