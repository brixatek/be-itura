using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetEmployees;

public sealed record GetEmployeesQuery(Guid AccountId, Guid RequestingUserId, int Page, int PageSize)
    : IRequest<Result<PagedResult<CorporateEmployeeDto>>>;
