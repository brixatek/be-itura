using Itura.Auth.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : IRequest<Result<LoginResponseDto>>;
