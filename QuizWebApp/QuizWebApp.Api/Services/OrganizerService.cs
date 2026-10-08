using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.OrganizerHome;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Services;

public class OrganizerService : IOrganizerService
{
    private readonly QuizContext _dbContext;

    public OrganizerService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<OrganizerHomeSummaryDTO>> GetHomeSummaryAsync()
    {
        try
        {
            OrganizerHomeSummaryDTO? summary = await _dbContext.Topics
                .AsNoTracking()
                .OrderBy(topic => topic.Id)
                .Select(_ => new OrganizerHomeSummaryDTO
                (
                    _dbContext.Topics.Count(),
                    
                    _dbContext.Users.Count(user => user.Role == nameof(UserRole.Participant)),
                    
                    _dbContext.Users.Count(user => user.Role == nameof(UserRole.Participant) && user.IsApproved),
                    
                    _dbContext.Quizzes.Count(),
                    
                    _dbContext.Quizzes.Count(quiz => quiz.IsActive)
                ))
                .FirstOrDefaultAsync();
            
            return QuizApiResponse<OrganizerHomeSummaryDTO>.Success(summary 
                ?? new OrganizerHomeSummaryDTO(0, 0, 0, 0, 0));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<OrganizerHomeSummaryDTO>.Fail(exception.Message);
        }
    }
}
