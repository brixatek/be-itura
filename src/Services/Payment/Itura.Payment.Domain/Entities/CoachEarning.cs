using Itura.SharedKernel.Entities;

namespace Itura.Payment.Domain.Entities;

public sealed class CoachEarning : AuditableEntity
{
    public Guid CoachUserId { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid BookingId { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal CommissionRate { get; private set; }
    public decimal CommissionAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; } = "NGN";
    public bool IsPaid { get; private set; }
    public Guid? PayoutId { get; private set; }

    private CoachEarning() { }

    public static CoachEarning Create(
        Guid coachUserId, Guid sessionId, Guid bookingId,
        decimal grossAmount, decimal commissionRate, string currency = "NGN")
    {
        var commissionAmount = Math.Round(grossAmount * commissionRate, 2);
        var netAmount = grossAmount - commissionAmount;

        return new CoachEarning
        {
            Id = Guid.NewGuid(),
            CoachUserId = coachUserId,
            SessionId = sessionId,
            BookingId = bookingId,
            GrossAmount = grossAmount,
            CommissionRate = commissionRate,
            CommissionAmount = commissionAmount,
            NetAmount = netAmount,
            Currency = currency.ToUpperInvariant(),
            IsPaid = false
        };
    }

    public void MarkPaid(Guid payoutId)
    {
        IsPaid = true;
        PayoutId = payoutId;
        MarkUpdated();
    }
}
