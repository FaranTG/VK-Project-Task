using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class QuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/quizzes";

    public Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(int topicFilterId, PaginationDTO paginationData)
        => httpClient
            .GetAsync($"{ApiRoute}?{CreateFilterParameter(topicFilterId)}&{paginationData.CreatePaginationParameters()}")
            .ExtractQuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>();

    public Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetActiveBriefQuizzesAsync(int topicFilterId, PaginationDTO paginationData)
        => httpClient
            .GetAsync($"{ApiRoute}/available?{CreateFilterParameter(topicFilterId)}&{paginationData.CreatePaginationParameters()}")
            .ExtractQuizApiResponse<PagedInfo<QuizBriefInfoDTO>>();

    public Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id)
        => httpClient
            .GetAsync($"{ApiRoute}/{id}")
            .ExtractQuizApiResponse<QuizInfoDTO>();

    public Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id)
        => httpClient
            .GetAsync($"{ApiRoute}/{id}/title")
            .ExtractQuizApiResponse<QuizTitleInfoDTO>();

    public Task<QuizApiResponse<QuizInfoDTO>> AddQuizAsync(QuizSaveDTO newQuiz)
        => httpClient
            .PostAsJsonAsync(ApiRoute, newQuiz)
            .ExtractQuizApiResponse<QuizInfoDTO>();
    
    public Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuiz)
        => httpClient
            .PutAsJsonAsync($"{ApiRoute}/{id}", newQuiz)
            .ExtractQuizApiResponse();

    private static string CreateFilterParameter(int topicFilterId)
        => $"topicFilterId={topicFilterId}";
}
