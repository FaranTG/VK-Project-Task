using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;

namespace QuizWebApp.Frontend.Extensions;

public static class QuizApiResponseExtensions
{
    private const string NoResponseMessage = "No response from server.";
    
    public static async Task<QuizApiResponse> ExtractQuizApiResponse(this Task<HttpResponseMessage> messageTask)
    {
        HttpResponseMessage response = await messageTask;

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        
        return responseData ?? QuizApiResponse.Fail(NoResponseMessage);
    }

    public static async Task<QuizApiResponse<TData>> ExtractQuizApiResponse<TData>(this Task<HttpResponseMessage> messageTask)
    {
        HttpResponseMessage response = await messageTask;

        QuizApiResponse<TData>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<TData>>();
        
        return responseData ?? QuizApiResponse<TData>.Fail(NoResponseMessage);
    }
}
