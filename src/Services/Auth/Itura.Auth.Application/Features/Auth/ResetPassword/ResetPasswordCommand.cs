using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string ConfirmPassword) : IRequest<Result>;
