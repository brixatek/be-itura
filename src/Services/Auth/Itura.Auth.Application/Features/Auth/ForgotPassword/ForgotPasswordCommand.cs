using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
