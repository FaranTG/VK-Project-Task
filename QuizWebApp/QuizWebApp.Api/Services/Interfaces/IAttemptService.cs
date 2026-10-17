using QuizWebApp.Api.Data.DataEnums;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Attempt;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IAttemptService
{
    public const string NotFoundMessage = "Attempt not found.";

    Task<QuizApiResponse<PagedInfoArray<AttemptParticipantInfoDTO>>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData);

    Task<QuizApiResponse<PagedInfoArray<AttemptQuizInfoDTO>>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData);

    Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId, int participantId);

    Task<QuizApiResponse<QuestionInfoDTO>> GetNextQuizQuestionAsync(int attemptId, int participantId);

    Task<QuizApiResponse> SaveQuestionResponseAsync(QuestionResponseSaveDTO responseData, int participantId);

    Task<QuizApiResponse> SubmitQuizAsync(int attemptId, AttemptStatus quitStatus, int participantId);
}