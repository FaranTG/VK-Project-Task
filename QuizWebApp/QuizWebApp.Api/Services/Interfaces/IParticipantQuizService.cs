using QuizWebApp.Api.Data.DataEnums;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IParticipantQuizService
{
    public const string NotFoundMessage = "Attempt not found.";

    Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter);

    Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId, int participantId);

    Task<QuizApiResponse<QuestionInfoDTO>> GetNextQuizQuestionAsync(int attemptId, int participantId);

    Task<QuizApiResponse> SaveQuestionResponseAsync(QuestionResponseSaveDTO responseData, int participantId);

    Task<QuizApiResponse> SubmitQuizAsync(int attemptId, ParticipantQuizStatus quitStatus, int participantId);
}