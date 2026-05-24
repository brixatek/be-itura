using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.VerifyEmail;

public sealed record ResendVerificationCommand(string Email) : IRequest<Result>;
