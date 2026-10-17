using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IQuizService
{
    public const string NotFoundMessage = "Quiz not found.";

    Task<QuizApiResponse<QuizSummaryInfoDTO[]>> GetQuizzesAsync();

    Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter);

    Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id);

    Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id);

    Task<QuizApiResponse<QuizInfoDTO>> CreateQuizAsync(QuizSaveDTO newQuizData);

    Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuizData);
}
