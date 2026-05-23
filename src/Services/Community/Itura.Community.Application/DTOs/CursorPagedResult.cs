namespace Itura.Community.Application.DTOs;

public sealed record CursorPagedResult<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    bool HasMore);
