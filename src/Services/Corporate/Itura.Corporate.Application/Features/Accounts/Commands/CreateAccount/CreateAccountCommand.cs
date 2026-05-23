using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.CreateAccount;

public sealed record CreateAccountCommand(
    string CompanyName,
    Guid AdminUserId,
    string SubscriptionTier,
    int MaxEmployees,
    string? Industry,
    string? LogoUrl,
    string? Website) : IRequest<Result<CorporateAccountDto>>;
