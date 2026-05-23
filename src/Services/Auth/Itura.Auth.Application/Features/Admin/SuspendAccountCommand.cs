using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Admin;

public sealed record SuspendAccountCommand(Guid AccountId, string? Reason) : IRequest<Result>;
public sealed record RestoreAccountCommand(Guid AccountId) : IRequest<Result>;
