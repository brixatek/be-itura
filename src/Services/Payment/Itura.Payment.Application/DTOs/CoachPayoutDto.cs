namespace Itura.Payment.Application.DTOs;

public sealed record CoachEarningsSummaryDto(
    decimal TotalEarnings,
    decimal UnpaidEarnings,
    string Currency);

public sealed record CoachPayoutDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    string? TransferReference,
    string? FailureReason,
    DateTime? ProcessedAt,
    DateTime CreatedAt);
