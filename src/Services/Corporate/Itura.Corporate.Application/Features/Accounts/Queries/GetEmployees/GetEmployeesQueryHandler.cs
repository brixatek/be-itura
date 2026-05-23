using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Domain.Entities;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetEmployees;

internal sealed class GetEmployeesQueryHandler(
    ICorporateAccountRepository accountRepository,
    ICorporateEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeesQuery, Result<PagedResult<CorporateEmployeeDto>>>
{
    public async Task<Result<PagedResult<CorporateEmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure<PagedResult<CorporateEmployeeDto>>(Error.NotFound("CorporateAccount", request.AccountId));

        if (account.AdminUserId != request.RequestingUserId)
            return Result.Failure<PagedResult<CorporateEmployeeDto>>(Error.Forbidden("Only the account admin can view employees."));

        var paged = await employeeRepository.GetByAccountIdAsync(request.AccountId, request.Page, request.PageSize, cancellationToken);
        var dtos = paged.Items.Select(ToDto).ToList();
        return new PagedResult<CorporateEmployeeDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    internal static CorporateEmployeeDto ToDto(CorporateEmployee e) =>
        new(e.Id, e.CorporateAccountId, e.UserId, e.Role.ToString(), e.IsActive, e.CreatedAt);
}
