using Itura.Booking.Application.Common.Interfaces;
using System.Text;

namespace Itura.Booking.Infrastructure.Services;

internal sealed class IcsCalendarService : ICalendarService
{
    public string GenerateIcs(
        Guid bookingId,
        DateTime scheduledAt,
        int durationMinutes,
        string summary,
        string description)
    {
        var utcStart = scheduledAt.Kind == DateTimeKind.Utc
            ? scheduledAt
            : scheduledAt.ToUniversalTime();
        var utcEnd = utcStart.AddMinutes(durationMinutes);
        var stamp = DateTime.UtcNow;

        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//Itura//Wellness//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:REQUEST");
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{bookingId:N}@itura.app");
        sb.AppendLine($"DTSTAMP:{stamp:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTSTART:{utcStart:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTEND:{utcEnd:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"SUMMARY:{EscapeIcs(summary)}");
        sb.AppendLine($"DESCRIPTION:{EscapeIcs(description)}");
        sb.AppendLine("STATUS:CONFIRMED");
        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString();
    }

    private static string EscapeIcs(string value) =>
        value.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,").Replace("\n", "\\n");
}
