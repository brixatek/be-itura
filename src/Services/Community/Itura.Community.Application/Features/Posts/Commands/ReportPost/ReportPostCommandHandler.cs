using Itura.Community.Application.Common.Interfaces;
using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Community.Application.Features.Posts.Commands.ReportPost;

internal sealed class ReportPostCommandHandler(
    ICommunityPostRepository postRepository,
    IPostReportRepository reportRepository,
    ICommunityUnitOfWork unitOfWork)
    : IRequestHandler<ReportPostCommand, Result>
{
    public async Task<Result> Handle(ReportPostCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure(Error.Validation("PostReport.Reason", "Reason is required."));

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return Result.Failure(Error.NotFound("PostReport.PostNotFound", "Post not found."));

        var alreadyReported = await reportRepository.ExistsAsync(request.PostId, request.ReporterUserId, cancellationToken);
        if (alreadyReported)
            return Result.Failure(Error.Conflict("PostReport.AlreadyReported", "You have already reported this post."));

        var report = PostReport.Create(request.PostId, request.ReporterUserId, request.Reason);
        await reportRepository.AddAsync(report, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
