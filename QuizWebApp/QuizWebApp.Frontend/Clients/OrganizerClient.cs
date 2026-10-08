using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Frontend.Clients;

public class OrganizerClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/organizer";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync()
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/summary");

        QuizApiResponse<OrganizerHomeSummaryDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<OrganizerHomeSummaryDTO>>();
        
        return responseData
            ?? QuizApiResponse<OrganizerHomeSummaryDTO>.Fail(NoResponseMessage);
    }
}
