using Itura.Auth.Domain.Enums;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Admin;

public sealed record AdminAccountDto(
    Guid Id,
    string Email,
    string Role,
    string Status,
    bool EmailVerified,
    DateTime? LastLoginAt,
    DateTime CreatedAt);

public sealed record SearchAccountsQuery(
    string? Search,
    string? Status,
    string? Role,
    DateTime? RegisteredFrom,
    DateTime? RegisteredTo,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<AdminAccountDto>>>;

public sealed record GetDashboardOverviewQuery() : IRequest<Result<DashboardOverviewDto>>;

public sealed record DashboardOverviewDto(
    int TotalUsers,
    int ActiveUsers,
    int SuspendedUsers,
    int NewUsersToday,
    int NewUsersThisWeek);
