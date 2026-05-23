using Itura.SharedKernel.Entities;

namespace Itura.Payment.Domain.Entities;

public sealed class CoachPayout : AuditableEntity
{
    public Guid CoachUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "NGN";
    public string Status { get; private set; } = "pending"; // pending, processing, success, failed
    public string? TransferReference { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private CoachPayout() { }

    public static CoachPayout Create(Guid coachUserId, decimal amount, string currency = "NGN")
    {
        return new CoachPayout
        {
            Id = Guid.NewGuid(),
            CoachUserId = coachUserId,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Status = "pending"
        };
    }

    public void MarkProcessing(string transferReference)
    {
        Status = "processing";
        TransferReference = transferReference;
        MarkUpdated();
    }

    public void MarkSuccess()
    {
        Status = "success";
        ProcessedAt = DateTime.UtcNow;
        MarkUpdated();
    }

    public void MarkFailed(string reason)
    {
        Status = "failed";
        FailureReason = reason;
        MarkUpdated();
    }
}
