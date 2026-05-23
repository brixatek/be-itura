using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.DeleteAccount;

public sealed record DeleteAccountCommand(Guid AccountId) : IRequest<Result>;
