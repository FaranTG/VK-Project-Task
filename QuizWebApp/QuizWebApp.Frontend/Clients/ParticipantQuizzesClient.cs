using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class ParticipantQuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/participant";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/available-quizzes?topicIdFilter={topicIdFilter}");

        QuizApiResponse<QuizBriefInfoDTO[]>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<QuizBriefInfoDTO[]>>();
        
        return responseData
            ?? QuizApiResponse<QuizBriefInfoDTO[]>.Fail(NoResponseMessage);
    }
}
