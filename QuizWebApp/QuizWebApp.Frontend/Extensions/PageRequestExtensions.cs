using Microsoft.AspNetCore.Components.QuickGrid;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.Interfaces;

namespace QuizWebApp.Frontend.Extensions;

public static class PageRequestExtensions
{
    public static async Task<GridItemsProviderResult<TData>> HandleGridRequestAsync<TData>
    (
        this GridItemsProviderRequest<TData> request,
        IAppState appState,
        string loaderMessage,
        int defaultPageSize,
        Func<PaginationDTO, Task<QuizApiResponse<PagedInfo<TData>>>> networkCall,
        Action<string?> onError
    )
    {
        appState.ShowLoader(loaderMessage);
        try
        {
            PaginationDTO paginationData = request.CreatePagination(defaultPageSize);

            QuizApiResponse<PagedInfo<TData>> response = await networkCall(paginationData);

            if (response.IsSuccess)
            {
                PagedInfo<TData> pagedInfo = response.Data!;
                return GridItemsProviderResult.From(pagedInfo.DTOs, pagedInfo.TotalCount);
            }

            onError(response.ErrorMessage);
            
            return GridItemsProviderResult.From(Array.Empty<TData>(), 0);
        }
        finally
        {
            appState.HideLoader();
        }
    }
}
