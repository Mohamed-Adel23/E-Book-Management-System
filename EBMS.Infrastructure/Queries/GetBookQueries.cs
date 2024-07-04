namespace EBMS.Infrastructure.Queries
{
    public record GetBookQueries(
        string? query, 
        string? sortColumn, 
        string? sortOrder,
        int Page,
        int PageSize
        );
}
