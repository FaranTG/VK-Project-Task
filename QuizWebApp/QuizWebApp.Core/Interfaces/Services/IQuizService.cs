using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IQuizService
{
    public const string NotFoundMessage = "Quiz not found.";

    Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(PaginationDTO paginationData, int topicFilterId);

    Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetAvailableQuizzesAsync(PaginationDTO paginationData, int topicFilterId);

    Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id);

    Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id);

    Task<QuizApiResponse<QuizInfoDTO>> CreateQuizAsync(QuizSaveDTO newQuizData);

    Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuizData);
}
