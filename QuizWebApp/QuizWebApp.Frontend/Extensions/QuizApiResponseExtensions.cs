using System.Net;
using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;

namespace QuizWebApp.Frontend.Extensions;

public static class QuizApiResponseExtensions
{
    private const string NoResponseMessage = "No response from server.";

    public static Task<QuizApiResponse> ExtractQuizApiResponse(this Task<HttpResponseMessage> messageTask)
        => CoreExtractResponseAsync(messageTask, QuizApiResponse.Fail(NoResponseMessage));

    public static Task<QuizApiResponse<TData>> ExtractQuizApiResponse<TData> (this Task<HttpResponseMessage> messageTask)
        => CoreExtractResponseAsync(messageTask, QuizApiResponse<TData>.Fail(NoResponseMessage));

    private static async Task<TResponse> CoreExtractResponseAsync<TResponse>
    (
        Task<HttpResponseMessage> messageTask,
        TResponse failFallback
    ) 
        where TResponse : QuizApiResponse
    {
        HttpResponseMessage response = await messageTask;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return failFallback;
        }
        
        TResponse? responseData = await response.Content
            .ReadFromJsonAsync<TResponse>();
            
        return responseData ?? failFallback;
    }
}
