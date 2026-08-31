using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class QuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/quizzes";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetQuizzesAsync()
    {
        HttpResponseMessage response = await httpClient.GetAsync(ApiRoute);

        QuizApiResponse<QuizBriefInfoDTO[]>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizBriefInfoDTO[]>>();
        
        return responseData
            ?? QuizApiResponse<QuizBriefInfoDTO[]>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/{id}");

        QuizApiResponse<QuizInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizInfoDTO>>();
        return responseData
            ?? QuizApiResponse<QuizInfoDTO>.Fail(NoResponseMessage);
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
}
