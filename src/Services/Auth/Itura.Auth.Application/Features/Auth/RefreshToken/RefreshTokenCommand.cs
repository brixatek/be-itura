using Itura.Auth.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken,
    string? DeviceInfo,
    string? IpAddress) : IRequest<Result<AuthTokensDto>>;
