namespace Itura.SharedKernel.Results;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string entity, object id) =>
        new($"{entity}.NotFound", $"{entity} with id '{id}' was not found.");

    public static Error Conflict(string code, string message) => new(code, message);
    public static Error Validation(string code, string message) => new(code, message);
    public static Error Unauthorized(string message = "Unauthorized.") => new("Auth.Unauthorized", message);
    public static Error Forbidden(string message = "Forbidden.") => new("Auth.Forbidden", message);
}
