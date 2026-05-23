using Itura.Media.Application.Common.Interfaces;
using Itura.Media.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Commands.DeleteMedia;

internal sealed class DeleteMediaCommandHandler(
    IMediaAssetRepository repository,
    IMediaUnitOfWork unitOfWork,
    IFileStorageService fileStorage)
    : IRequestHandler<DeleteMediaCommand, Result>
{
    public async Task<Result> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var asset = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return Result.Failure(Error.NotFound("MediaAsset", request.Id));

        if (asset.UploaderUserId != request.RequestingUserId)
            return Result.Failure(Error.Forbidden("Only the uploader can delete this asset."));

        await fileStorage.DeleteAsync(asset.StoredFileName, cancellationToken);

        asset.Delete();
        repository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
