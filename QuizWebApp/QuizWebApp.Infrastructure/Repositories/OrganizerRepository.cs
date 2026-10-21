using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Shared.DTOs.OrganizerHome;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Infrastructure.Repositories;

public class OrganizerRepository : IOrganizerRepository
{
    private readonly QuizContext _dbContext;

    public OrganizerRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrganizerHomeSummaryDTO> GetHomeSummaryAsync()
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

        return summary ?? new OrganizerHomeSummaryDTO(0, 0, 0, 0, 0);
    }
}
