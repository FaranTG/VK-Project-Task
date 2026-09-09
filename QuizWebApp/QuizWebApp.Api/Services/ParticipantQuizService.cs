using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services;

public class ParticipantQuizService : IParticipantQuizService
{
    private readonly QuizContext _dbContext;

    public ParticipantQuizService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter)
    {
        try
        {
            IQueryable<Quiz> query = _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz => quiz.IsActive);
            
            if (topicIdFilter > 0)
            {
                query = query.Where(quiz => quiz.TopicId == topicIdFilter);
            }
            
            QuizBriefInfoDTO[] quizzes = await query
                .Select
                (
                    quiz => new QuizBriefInfoDTO
                    (
                        quiz.Id,
                        quiz.Name,
                        quiz.TopicId,
                        quiz.Topic!.Name,
                        quiz.QuestionsNumber,
                        quiz.TimeInMinutes
                    )
                )
                .ToArrayAsync();
            
            return QuizApiResponse<QuizBriefInfoDTO[]>.Success(quizzes);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizBriefInfoDTO[]>.Fail(exception.Message);
        }
    }
}