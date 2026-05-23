using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.UpdateAccount;

public sealed record UpdateAccountCommand(
    Guid Id,
    Guid AdminUserId,
    string CompanyName,
    string? Industry,
    string SubscriptionTier,
    int MaxEmployees,
    string? LogoUrl,
    string? Website) : IRequest<Result<CorporateAccountDto>>;
