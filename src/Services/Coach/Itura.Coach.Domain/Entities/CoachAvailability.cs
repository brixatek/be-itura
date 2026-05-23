using Itura.SharedKernel.Entities;

namespace Itura.Coach.Domain.Entities;

public sealed class CoachAvailability : AuditableEntity
{
    public Guid CoachUserId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int SlotDurationMinutes { get; private set; }
    public string Timezone { get; private set; } = "UTC";

    private CoachAvailability() { }

    public static CoachAvailability Create(
        Guid coachUserId, DayOfWeek dayOfWeek,
        TimeOnly startTime, TimeOnly endTime,
        int slotDurationMinutes = 60, string timezone = "UTC")
    {
        return new CoachAvailability
        {
            Id = Guid.NewGuid(),
            CoachUserId = coachUserId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            SlotDurationMinutes = slotDurationMinutes,
            Timezone = timezone
        };
    }
}
