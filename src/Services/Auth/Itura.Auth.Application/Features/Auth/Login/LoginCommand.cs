using Itura.Auth.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? DeviceInfo,
    string? IpAddress) : IRequest<Result<LoginResponseDto>>;
