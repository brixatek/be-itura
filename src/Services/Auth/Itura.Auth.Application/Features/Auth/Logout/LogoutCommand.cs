using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Logout;

public sealed record LogoutCommand(
    Guid AccountId,
    string Jti,
    string RefreshToken,
    bool LogoutAllDevices = false) : IRequest<Result>;
