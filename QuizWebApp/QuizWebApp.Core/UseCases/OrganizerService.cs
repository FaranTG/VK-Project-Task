using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;

namespace QuizWebApp.Core.UseCases;

public class OrganizerService : IOrganizerService
{
    private readonly IOrganizerRepository _organizerRepository;

    public OrganizerService(IOrganizerRepository organizerRepository)
    {
        _organizerRepository = organizerRepository;
    }

    public async Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync()
    {
        try
        {
            OrganizerHomeSummaryDTO summary = await _organizerRepository
                .GetHomeSummaryAsync();
            
            return QuizApiResponse<OrganizerHomeSummaryDTO>.Success(summary);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<OrganizerHomeSummaryDTO>.Fail(exception.Message);
        }
    }
}
