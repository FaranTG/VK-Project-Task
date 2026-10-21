using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IOrganizerService
{
    Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync();
}
