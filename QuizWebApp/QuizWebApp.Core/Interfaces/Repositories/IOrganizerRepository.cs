using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface IOrganizerRepository
{
    Task<OrganizerHomeSummaryDTO> GetHomeSummaryAsync();
}