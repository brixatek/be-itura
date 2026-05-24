using Itura.Journal.Application.DTOs;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.Templates;

internal sealed class GetJournalTemplatesQueryHandler(IJournalTemplateRepository repository)
    : IRequestHandler<GetJournalTemplatesQuery, Result<List<JournalTemplateDto>>>
{
    public async Task<Result<List<JournalTemplateDto>>> Handle(
        GetJournalTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = string.IsNullOrWhiteSpace(request.Category)
            ? await repository.GetAllAsync(cancellationToken)
            : await repository.GetByCategoryAsync(request.Category, cancellationToken);

        var dtos = templates.Select(t => new JournalTemplateDto(
            t.Id, t.Name, t.Description, t.Prompt, t.Category)).ToList();

        return Result.Success(dtos);
    }
}
