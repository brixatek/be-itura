using Itura.Payment.Domain.Enums;
using Itura.Payment.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Payment.Domain.Entities;

public sealed class PaymentRecord : AggregateRoot
{
    public Guid BookingId { get; private set; }
    public Guid PayerUserId { get; private set; }
    public Guid PayeeUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public PaymentStatus Status { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? TransactionReference { get; private set; }
    public string? FailureReason { get; private set; }

    private PaymentRecord() { }

    public static Result<PaymentRecord> Initiate(
        Guid bookingId, Guid payerUserId, Guid payeeUserId,
        decimal amount, string currency, string? paymentMethod = null)
    {
        if (payerUserId == payeeUserId)
            return Result.Failure<PaymentRecord>(Error.Validation("Payment.SelfPayment", "Payer and payee cannot be the same user."));
        if (amount <= 0)
            return Result.Failure<PaymentRecord>(Error.Validation("Payment.Amount", "Payment amount must be greater than zero."));
        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<PaymentRecord>(Error.Validation("Payment.Currency", "Currency is required."));

        var record = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            PayerUserId = payerUserId,
            PayeeUserId = payeeUserId,
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
            Status = PaymentStatus.Pending,
            PaymentMethod = paymentMethod
        };

        record.RaiseDomainEvent(new PaymentInitiatedDomainEvent(
            record.Id, bookingId, payerUserId, payeeUserId, amount, record.Currency));

        return Result.Success(record);
    }

    public Result Complete(string? transactionReference = null)
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(Error.Validation("Payment.Complete", "Only pending payments can be completed."));

        Status = PaymentStatus.Completed;
        TransactionReference = transactionReference;
        MarkUpdated();
        RaiseDomainEvent(new PaymentCompletedDomainEvent(
            Id, BookingId, PayerUserId, PayeeUserId, Amount, Currency, transactionReference));
        return Result.Success();
    }

    public Result Fail(string? reason = null)
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(Error.Validation("Payment.Fail", "Only pending payments can be marked as failed."));

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        MarkUpdated();
        RaiseDomainEvent(new PaymentFailedDomainEvent(
            Id, BookingId, PayerUserId, Amount, Currency, reason));
        return Result.Success();
    }

    public Result Refund(decimal? refundAmount = null)
    {
        if (Status != PaymentStatus.Completed)
            return Result.Failure(Error.Validation("Payment.Refund", "Only completed payments can be refunded."));

        var actualRefund = refundAmount ?? Amount;
        if (actualRefund <= 0 || actualRefund > Amount)
            return Result.Failure(Error.Validation("Payment.RefundAmount", "Refund amount must be between 0 and the original payment amount."));

        Status = PaymentStatus.Refunded;
        MarkUpdated();
        RaiseDomainEvent(new PaymentRefundedDomainEvent(
            Id, BookingId, PayerUserId, PayeeUserId, actualRefund, Currency));
        return Result.Success();
    }

    public void AssignReference(string reference)
    {
        TransactionReference = reference;
        MarkUpdated();
    }
}
