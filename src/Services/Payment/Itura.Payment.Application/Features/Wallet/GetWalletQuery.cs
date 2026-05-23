using Itura.Payment.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Payment.Application.Features.Wallet;

public sealed record GetWalletQuery(Guid UserId) : IRequest<Result<WalletDto>>;
