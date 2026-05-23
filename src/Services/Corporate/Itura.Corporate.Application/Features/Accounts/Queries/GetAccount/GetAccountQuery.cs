using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetAccount;

public sealed record GetAccountQuery(Guid Id) : IRequest<Result<CorporateAccountDto>>;
