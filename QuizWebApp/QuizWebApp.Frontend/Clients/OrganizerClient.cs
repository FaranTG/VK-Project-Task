using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Frontend.Clients;

public class OrganizerClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/organizer";

    public Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync()
        => httpClient
            .GetAsync($"{ApiRoute}/summary")
            .ExtractQuizApiResponse<OrganizerHomeSummaryDTO>();
}
