using Itura.SharedKernel.Entities;

namespace Itura.User.Domain.Entities;

public sealed class WellnessGoal : BaseEntity
{
    public Guid UserProfileId { get; private set; }
    public string Goal { get; private set; } = string.Empty;
    public int Priority { get; private set; }

    private WellnessGoal() { }

    public static WellnessGoal Create(Guid profileId, string goal, int priority) =>
        new() { UserProfileId = profileId, Goal = goal, Priority = priority };
}
