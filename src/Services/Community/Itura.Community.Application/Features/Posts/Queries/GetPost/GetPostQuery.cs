using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPost;

public sealed record GetPostQuery(Guid Id) : IRequest<Result<CommunityPostDto>>;
