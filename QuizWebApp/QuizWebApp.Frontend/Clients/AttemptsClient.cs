using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Attempt;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;

namespace QuizWebApp.Frontend.Clients;

public class AttemptsClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/attempts";
    private const string OrganizerApiRoute = ApiRoute + "/organizer";
    private const string ParticipantApiRoute = ApiRoute + "/participant";
    private const string QuizApiRoute = ParticipantApiRoute + "/take-quiz";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData)
    {
        string requestUrl = $"{OrganizerApiRoute}/quiz-info/{quizId}?{paginationData.CreatePaginationParameters()}";
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>? responseData = await response.Content
            .ReadFromJsonAsync<QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>>();
        
        return responseData
            ?? QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>> GetParticipantAttemptsAsync(PaginationDTO paginationData)
    {
        string requestUrl = $"{ParticipantApiRoute}?{paginationData.CreatePaginationParameters()}";
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>? responseData = await response.Content
            .ReadFromJsonAsync<QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>>();
        
        return responseData
            ?? QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>.Fail(NoResponseMessage);
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
