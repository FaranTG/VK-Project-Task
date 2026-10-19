using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class QuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/quizzes";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(int topicFilterId, PaginationDTO paginationData)
    {
        string requestUrl = $"{ApiRoute}?{CreateFilterParameter(topicFilterId)}&{paginationData.CreatePaginationParameters()}";
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>? responseData = await response.Content
            .ReadFromJsonAsync<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>>();
        
        return responseData
            ?? QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetActiveBriefQuizzesAsync(int topicFilterId, PaginationDTO paginationData)
    {
        string requestUrl = $"{ApiRoute}/active/brief?{CreateFilterParameter(topicFilterId)}&{paginationData.CreatePaginationParameters()}";
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>? responseData = await response.Content
            .ReadFromJsonAsync<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>>();
        
        return responseData
            ?? QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/{id}");

        QuizApiResponse<QuizInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizInfoDTO>>();
        return responseData
            ?? QuizApiResponse<QuizInfoDTO>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/{id}/title");

        QuizApiResponse<QuizTitleInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizTitleInfoDTO>>();
        return responseData
            ?? QuizApiResponse<QuizTitleInfoDTO>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<QuizInfoDTO>> AddQuizAsync(QuizSaveDTO newQuiz)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(ApiRoute, newQuiz);

        QuizApiResponse<QuizInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizInfoDTO>>();
        return responseData
            ?? QuizApiResponse<QuizInfoDTO>.Fail(NoResponseMessage);
    }
    
    public async Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuiz)
    {
        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{ApiRoute}/{id}", newQuiz);

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }

    private static string CreateFilterParameter(int topicFilterId)
        => $"topicFilterId={topicFilterId}";
}
