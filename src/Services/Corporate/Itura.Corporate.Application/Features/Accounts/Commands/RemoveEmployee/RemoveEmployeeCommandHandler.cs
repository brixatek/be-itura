using Itura.Corporate.Application.Common.Interfaces;
using Itura.Corporate.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.RemoveEmployee;

internal sealed class RemoveEmployeeCommandHandler(
    ICorporateAccountRepository accountRepository,
    ICorporateEmployeeRepository employeeRepository,
    ICorporateUnitOfWork unitOfWork)
    : IRequestHandler<RemoveEmployeeCommand, Result>
{
    public async Task<Result> Handle(RemoveEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure(Error.NotFound("CorporateEmployee", request.EmployeeId));

        var account = await accountRepository.GetByIdAsync(employee.CorporateAccountId, cancellationToken);
        if (account is null || account.AdminUserId != request.AdminUserId)
            return Result.Failure(Error.Forbidden("Only the account admin can remove employees."));

        employee.Delete();
        employeeRepository.Update(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
