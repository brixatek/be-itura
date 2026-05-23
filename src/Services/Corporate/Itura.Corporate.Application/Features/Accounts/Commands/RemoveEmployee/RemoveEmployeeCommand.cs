using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Commands.RemoveEmployee;

public sealed record RemoveEmployeeCommand(Guid EmployeeId, Guid AdminUserId) : IRequest<Result>;
