using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IParticipantQuizService
{
    Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter);
}