using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class QuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/quizzes";

    public Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData)
        => httpClient
            .GetAsync($"{ApiRoute}?{filterData.CreateFilterParameters(paginationData)}")
            .ExtractQuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>();

    public Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetActiveBriefQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData)
        => httpClient
            .GetAsync($"{ApiRoute}/available?{filterData.CreateFilterParameters(paginationData)}")
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
}
