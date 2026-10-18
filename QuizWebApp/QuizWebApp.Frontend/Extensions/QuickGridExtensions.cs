using Microsoft.AspNetCore.Components.QuickGrid;
using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Frontend.Extensions;

public static class QuickGridExtensions
{
    public static PaginationDTO CreatePagination<T>(this GridItemsProviderRequest<T> request, int defaultPageSize)
    {
        int pageSize = request.Count ?? defaultPageSize;
        int pageNumber = request.StartIndex / pageSize + 1;
        return new PaginationDTO(pageNumber, pageSize);
    }
}
