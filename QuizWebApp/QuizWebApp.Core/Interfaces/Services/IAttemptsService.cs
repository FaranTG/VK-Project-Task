using QuizWebApp.Shared.Enums;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Attempt;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IAttemptsService
{
    public const string NotFoundMessage = "Attempt not found.";

    Task<QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData);

    Task<QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData);

    Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId, int participantId);

    Task<QuizApiResponse<QuestionInfoDTO>> GetNextQuizQuestionAsync(int attemptId, int participantId);

    Task<QuizApiResponse> SaveQuestionResponseAsync(QuestionResponseSaveDTO responseData, int participantId);

    Task<QuizApiResponse> SubmitQuizAsync(int attemptId, AttemptStatus quitStatus, int participantId);
}