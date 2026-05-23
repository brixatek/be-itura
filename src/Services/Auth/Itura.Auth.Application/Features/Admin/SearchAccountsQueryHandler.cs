using Itura.Auth.Domain.Enums;
using Itura.Auth.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Admin;

internal sealed class SearchAccountsQueryHandler(IAccountRepository repository)
    : IRequestHandler<SearchAccountsQuery, Result<PagedResult<AdminAccountDto>>>,
      IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
{
    public async Task<Result<PagedResult<AdminAccountDto>>> Handle(
        SearchAccountsQuery request, CancellationToken cancellationToken)
    {
        AccountStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<AccountStatus>(request.Status, true, out var parsedStatus))
            status = parsedStatus;

        UserRole? role = null;
        if (!string.IsNullOrWhiteSpace(request.Role) &&
            Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
            role = parsedRole;

        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var paged = await repository.SearchAsync(
            request.Search, status, role,
            request.RegisteredFrom, request.RegisteredTo,
            request.Page, pageSize, cancellationToken);

        var dtos = paged.Items.Select(a => new AdminAccountDto(
            a.Id, a.Email, a.Role.ToString(), a.Status.ToString(),
            a.EmailVerified, a.LastLoginAt, a.CreatedAt)).ToList();

        return new PagedResult<AdminAccountDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<Result<DashboardOverviewDto>> Handle(
        GetDashboardOverviewQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var weekStart = now.AddDays(-7);

        var totalTask = repository.CountAsync(null, cancellationToken);
        var activeTask = repository.CountAsync(AccountStatus.Active, cancellationToken);
        var suspendedTask = repository.CountAsync(AccountStatus.Suspended, cancellationToken);

        await Task.WhenAll(totalTask, activeTask, suspendedTask);

        var newToday = await repository.SearchAsync(
            null, null, null, todayStart, null, 1, 1, cancellationToken);
        var newThisWeek = await repository.SearchAsync(
            null, null, null, weekStart, null, 1, 1, cancellationToken);

        return new DashboardOverviewDto(
            totalTask.Result,
            activeTask.Result,
            suspendedTask.Result,
            newToday.TotalCount,
            newThisWeek.TotalCount);
    }
}
