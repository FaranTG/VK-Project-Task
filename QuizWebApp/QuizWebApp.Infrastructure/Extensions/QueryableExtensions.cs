using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationDTO paginationData)
        => query
            .Skip((paginationData.PageNumber - 1) * paginationData.PageSize)
            .Take(paginationData.PageSize);
}
