namespace Itura.User.Application.Common.Interfaces;

public interface IBadgeEvaluationService
{
    Task EvaluateAndAwardAsync(Guid userId, string trigger, CancellationToken ct = default);
}
