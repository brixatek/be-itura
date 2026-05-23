using Itura.Auth.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.OAuth;

public sealed record GoogleOAuthCommand(
    string Code,
    string RedirectUri,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<Result<LoginResponseDto>>;
