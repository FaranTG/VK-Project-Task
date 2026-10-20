using Microsoft.AspNetCore.Components.QuickGrid;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.Interfaces;

namespace QuizWebApp.Frontend.Extensions;

public static class PageRequestExtensions
{
    public static async Task<QuizApiResponse> CastToBaseAsync<TData>(this Task<QuizApiResponse<TData>> task)
        => await task;
    
    /// <summary>
    /// Обрабатывает запросы к API, которые возвращают данные для QuickGrid.
    /// Управляет Loader, из Task извлекает QuizApiResponse.
    /// Если QuizApiResponse.IsSuccess, вызывает onSuccess,
    /// иначе onError.
    /// </summary>
    public static async Task<GridItemsProviderResult<TData>> HandleGridRequestAsync<TData>
    (
        this Task<QuizApiResponse<PagedInfo<TData>>> responseTask,
        IAppState appState,
        string loaderMessage,
        Action<string>? onError = null
    )
    {
        appState.ShowLoader(loaderMessage);
        try
        {
            QuizApiResponse<PagedInfo<TData>> response = await responseTask;

            if (response.IsSuccess)
            {
                PagedInfo<TData> pagedInfo = response.Data!;
                return GridItemsProviderResult.From(pagedInfo.DTOs, pagedInfo.TotalCount);
            }

            onError?.Invoke(response.ErrorMessage!);
            return GridItemsProviderResult.From(Array.Empty<TData>(), 0);
        }
        finally
        {
            appState.HideLoader();
        }
    }

    /// <summary>
    /// Обрабатывает запросы к API, которые возвращают тело.
    /// Управляет Loader, из Task извлекает QuizApiResponse.
    /// Если QuizApiResponse.IsSuccess, вызывает onSuccess,
    /// иначе onError.
    /// </summary>
    public static Task<bool> HandleRequestAsync<TData>
    (
        this Task<QuizApiResponse<TData>> responseTask,
        IAppState appState,
        string loaderMessage,
        Action<TData>? onSuccess,
        Action<string>? onError
    )
        => CoreHandleRequestAsync
        (
            appState,
            loaderMessage,
            async () => await responseTask,
            response => onSuccess?.Invoke(((QuizApiResponse<TData>)response).Data!),
            onError
        );

    /// <summary>
    /// Обрабатывает запросы к API, которые ничего не возращают.
    /// Управляет Loader, из Task извлекает QuizApiResponse.
    /// Если QuizApiResponse.IsSuccess, вызывает onSuccess,
    /// иначе onError.
    /// </summary>
    public static Task<bool> HandleRequestAsync
    (
        this Task<QuizApiResponse> responseTask,
        IAppState appState,
        string loaderMessage,
        Action? onSuccess,
        Action<string>? onError
    )
        => CoreHandleRequestAsync
        (
            appState,
            loaderMessage,
            () => responseTask,
            _ => onSuccess?.Invoke(),
            onError
        );

    /// <summary>
    /// Внутреннее полиморфное ядро для управления лоадером и проверки базового статуса ответа.
    /// </summary>
    private static async Task<bool> CoreHandleRequestAsync
    (
        IAppState appState,
        string loaderMessage,
        Func<Task<QuizApiResponse>> responseTaskProvider,
        Action<QuizApiResponse> onSuccessWrapper,
        Action<string>? onError
    )
    {
        appState.ShowLoader(loaderMessage);
        try
        {
            QuizApiResponse response = await responseTaskProvider();

            if (response.IsFailure)
            {
                onError?.Invoke(response.ErrorMessage!);
                return false;
            }

            onSuccessWrapper(response);
            return true;
        }
        finally
        {
            appState.HideLoader();
        }
    }
}
