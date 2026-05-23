using Itura.Corporate.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Corporate.Application.Features.Accounts.Queries.GetMyAccount;

public sealed record GetMyAccountQuery(Guid AdminUserId) : IRequest<Result<CorporateAccountDto>>;
