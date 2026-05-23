using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Auth.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FullName,
    string Timezone) : IRequest<Result<Guid>>;
