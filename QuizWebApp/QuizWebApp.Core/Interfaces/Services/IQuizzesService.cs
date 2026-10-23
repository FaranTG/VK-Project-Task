using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IQuizzesService
{
    public const string NotFoundMessage = "Quiz not found.";

    Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData);

    Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetAvailableQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData);

    Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id);

    Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id);

    Task<QuizApiResponse<QuizInfoDTO>> CreateQuizAsync(QuizSaveDTO newQuizData);

    Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuizData);
}
