using Microsoft.AspNetCore.Components.QuickGrid;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.Interfaces;

namespace QuizWebApp.Frontend.Extensions;

public static class PageRequestExtensions
{
    /// <summary>
    /// Управляет Loader, получает response из networkCall, 
    /// на основе ответа выбрасывает onSuccess и true
    /// или onError и false
    /// </summary>
    public static async Task<bool> HandleRequestAsync<TData>
    (
        this IAppState appState, // Расширяем IAppState для красивого Fluent-вызова!
        string loaderMessage,
        Func<Task<QuizApiResponse<TData>>> networkCall,
        Action<TData> onSuccess,
        Action<string?> onError
    )
    {
        appState.ShowLoader(loaderMessage);
        try
        {
            QuizApiResponse<TData> response = await networkCall();

            if (response.IsFailure)
            {
                onError(response.ErrorMessage);
                return false;
            }

            onSuccess(response.Data!);
        }
        finally
        {
            appState.HideLoader();
        }

        return true;
    }

    /// <summary>
    /// Управляет Loader, получает response из networkCall, 
    /// на основе ответа выбрасывает onSuccess и true
    /// или onError и false
    /// </summary>
    public static async Task<bool> HandleRequestAsync
    (
        this IAppState appState,
        string loaderMessage,
        Func<Task<QuizApiResponse>> networkCall,
        Action onSuccess,
        Action<string?> onError
    )
    {
        appState.ShowLoader(loaderMessage);
        try
        {
            QuizApiResponse response = await networkCall();

            if (response.IsFailure)
            {
                onError(response.ErrorMessage);
                return false;
            }

            onSuccess();
        }
        finally
        {
            appState.HideLoader();
        }

        return true;
    }

    /// <summary>
    /// Управляет Loader, получает response из networkCall, 
    /// на основе ответа возвращает результат с записями и общим количеством
    /// или пустой результат
    /// </summary>
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
