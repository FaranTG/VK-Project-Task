using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class ParticipantQuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/participant";
    private const string QuizApiRoute = ApiRoute + "/take-quiz";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/available-quizzes?topicIdFilter={topicIdFilter}");

        QuizApiResponse<QuizBriefInfoDTO[]>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizBriefInfoDTO[]>>();
        
        return responseData
            ?? QuizApiResponse<QuizBriefInfoDTO[]>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{QuizApiRoute}/{quizId}/start", null as object);

        QuizApiResponse<int>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<int>>();
        return responseData
            ?? QuizApiResponse<int>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<QuestionInfoDTO>> GetQuizNextQuestionAsync(int attemptId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{QuizApiRoute}/{attemptId}/next-question");

        QuizApiResponse<QuestionInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuestionInfoDTO>>();
        return responseData
            ?? QuizApiResponse<QuestionInfoDTO>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse> SaveQuestionResponseAsync(int attemptId, QuestionResponseSaveDTO responseSaveDTO)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{QuizApiRoute}/{attemptId}/save-response", responseSaveDTO);

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }

    public Task<QuizApiResponse> CompleteQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "complete");
    
    public Task<QuizApiResponse> AutoSubmitQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "auto-submit");
    
    public Task<QuizApiResponse> ExitQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "exit");

    private async Task<QuizApiResponse> SubmitQuizAsync(int attemptId, string endpointName)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{QuizApiRoute}/{attemptId}/{endpointName}", null as object);

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }
}
