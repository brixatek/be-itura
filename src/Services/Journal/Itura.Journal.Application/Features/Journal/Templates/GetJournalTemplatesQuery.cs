using Itura.Journal.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.Templates;

public sealed record GetJournalTemplatesQuery(string? Category = null) : IRequest<Result<List<JournalTemplateDto>>>;
