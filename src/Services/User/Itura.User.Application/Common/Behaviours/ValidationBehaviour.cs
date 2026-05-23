using FluentValidation;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Common.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next();

        var errors = failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}").ToList();
        var message = string.Join("; ", errors);

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var errorObj = Error.Validation("Validation.Failed", message);
            var method = typeof(Result).GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
                .MakeGenericMethod(typeof(TResponse).GetGenericArguments()[0]);
            return (TResponse)method.Invoke(null, [errorObj])!;
        }

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(Error.Validation("Validation.Failed", message));

        throw new ValidationException(failures);
    }
}
