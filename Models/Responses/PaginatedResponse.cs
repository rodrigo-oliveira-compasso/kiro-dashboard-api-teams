namespace TeamsApi.Models.Responses;

public record PaginatedResponse<T>(
    List<T> Items,
    PaginationMetadata Pagination);

public record PaginationMetadata(
    int TotalItems,
    int CurrentPage,
    int PageSize,
    int TotalPages);
