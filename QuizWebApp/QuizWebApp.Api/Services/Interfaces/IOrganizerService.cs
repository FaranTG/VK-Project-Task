using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IOrganizerService
{
    Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync();
}
