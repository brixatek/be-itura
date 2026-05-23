using Itura.Community.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Queries.GetPostComments;

public sealed record GetPostCommentsQuery(Guid PostId, int Page, int PageSize) : IRequest<Result<PagedResult<PostCommentDto>>>;
