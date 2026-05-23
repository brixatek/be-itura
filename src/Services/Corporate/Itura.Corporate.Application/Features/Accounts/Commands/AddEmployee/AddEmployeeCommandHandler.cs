using Itura.Corporate.Application.Common.Interfaces;
using Itura.Corporate.Application.DTOs;
using Itura.Corporate.Application.Features.Accounts.Queries.GetEmployees;
using Itura.Corporate.Domain.Enums;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.AddEmployee;

internal sealed class AddEmployeeCommandHandler(
    ICorporateAccountRepository accountRepository,
    ICorporateEmployeeRepository employeeRepository,
    ICorporateUnitOfWork unitOfWork)
    : IRequestHandler<AddEmployeeCommand, Result<CorporateEmployeeDto>>
{
    public async Task<Result<CorporateEmployeeDto>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null)
            return Result.Failure<CorporateEmployeeDto>(Error.NotFound("CorporateAccount", request.AccountId));

        if (account.AdminUserId != request.AdminUserId)
            return Result.Failure<CorporateEmployeeDto>(Error.Forbidden("Only the account admin can add employees."));

        var existing = await employeeRepository.GetByAccountAndUserAsync(request.AccountId, request.EmployeeUserId, cancellationToken);
        if (existing is not null)
            return Result.Failure<CorporateEmployeeDto>(Error.Conflict("CorporateEmployee.Conflict", "This user is already a member of this account."));

        if (!Enum.TryParse<EmployeeRole>(request.Role, true, out var role))
            role = EmployeeRole.Member;

        var result = Domain.Entities.CorporateEmployee.Create(request.AccountId, request.EmployeeUserId, role);
        if (result.IsFailure)
            return Result.Failure<CorporateEmployeeDto>(result.Error);

        await employeeRepository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GetEmployeesQueryHandler.ToDto(result.Value);
    }
}
