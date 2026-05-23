using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.AddEmployee;

public sealed record AddEmployeeCommand(
    Guid AccountId,
    Guid AdminUserId,
    Guid EmployeeUserId,
    string Role = "Member") : IRequest<Result<CorporateEmployeeDto>>;
