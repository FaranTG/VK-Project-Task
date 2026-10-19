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

    public Task<QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData)
        => httpClient
            .GetAsync($"{OrganizerApiRoute}/quiz-info/{quizId}?{paginationData.CreatePaginationParameters()}")
            .ExtractQuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>();

    public Task<QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>> GetParticipantAttemptsAsync(PaginationDTO paginationData)
        => httpClient
            .GetAsync($"{ParticipantApiRoute}?{paginationData.CreatePaginationParameters()}")
            .ExtractQuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>();

    public Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId)
        => httpClient
            .PostAsJsonAsync($"{QuizApiRoute}/{quizId}/start", null as object)
            .ExtractQuizApiResponse<int>();

    public Task<QuizApiResponse<QuestionInfoDTO>> GetQuizNextQuestionAsync(int attemptId)
        => httpClient
            .GetAsync($"{QuizApiRoute}/{attemptId}/next-question")
            .ExtractQuizApiResponse<QuestionInfoDTO>();

    public Task<QuizApiResponse> SaveQuestionResponseAsync(int attemptId, QuestionResponseSaveDTO responseSaveDTO)
        => httpClient
            .PostAsJsonAsync($"{QuizApiRoute}/{attemptId}/save-response", responseSaveDTO)
            .ExtractQuizApiResponse();

    public Task<QuizApiResponse> CompleteQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "complete");
    
    public Task<QuizApiResponse> AutoSubmitQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "auto-submit");
    
    public Task<QuizApiResponse> ExitQuizAsync(int attemptId) =>
        SubmitQuizAsync(attemptId, "exit");

    private Task<QuizApiResponse> SubmitQuizAsync(int attemptId, string endpointName)
        => httpClient
            .PostAsJsonAsync($"{QuizApiRoute}/{attemptId}/{endpointName}", null as object)
            .ExtractQuizApiResponse();
}
