using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.ReportPost;

public sealed record ReportPostCommand(Guid PostId, Guid ReporterUserId, string Reason) : IRequest<Result>;
