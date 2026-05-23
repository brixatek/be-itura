using Itura.Corporate.Domain.Enums;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Corporate.Domain.Entities;

public sealed class CorporateEmployee : AggregateRoot
{
    public Guid CorporateAccountId { get; private set; }
    public Guid UserId { get; private set; }
    public EmployeeRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private CorporateEmployee() { }

    public static Result<CorporateEmployee> Create(Guid corporateAccountId, Guid userId, EmployeeRole role = EmployeeRole.Member)
    {
        if (corporateAccountId == Guid.Empty)
            return Result.Failure<CorporateEmployee>(Error.Validation("CorporateEmployee.AccountId", "Account is required."));
        if (userId == Guid.Empty)
            return Result.Failure<CorporateEmployee>(Error.Validation("CorporateEmployee.UserId", "User is required."));

        return new CorporateEmployee
        {
            CorporateAccountId = corporateAccountId,
            UserId = userId,
            Role = role,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
    public void Delete() => MarkDeleted();
}
