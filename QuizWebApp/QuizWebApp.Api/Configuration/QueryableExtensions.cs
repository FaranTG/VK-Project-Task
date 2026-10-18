using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Api.Configuration;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationDTO paginationData)
    {
        return query
            .Skip((paginationData.PageNumber - 1) * paginationData.PageSize)
            .Take(paginationData.PageSize);
    }
}
