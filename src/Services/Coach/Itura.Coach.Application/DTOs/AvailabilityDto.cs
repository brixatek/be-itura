namespace Itura.Coach.Application.DTOs;

public sealed record AvailabilityBlockDto(
    Guid Id,
    string DayOfWeek,
    string StartTime,
    string EndTime,
    int SlotDurationMinutes,
    string Timezone);

public sealed record TimeSlotDto(
    DateTime StartUtc,
    DateTime EndUtc,
    string StartLocal,
    string EndLocal);
