using QuizWebApp.Core.Entities;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface IQuizzesRepository
{
    Task<PagedInfo<Quiz>> GetQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData);
    
    Task<PagedInfo<Quiz>> GetAvailableQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData);
    
    Task<Quiz?> GetByIdWithQuestionsAndOptionsAsync(Guid id);
    
    Task<Quiz?> GetByIdAsync(Guid id);
    
    Task<bool> ExistsByNameAsync(string name);
    
    Task<bool> ExistsByNameWithDifferentIdAsync(string name, Guid id);
    
    Task DeleteQuestionsByQuizIdAsync(Guid quizId);
    
    Task AddAsync(Quiz quiz);
    
    Task UpdateAsync(Quiz quiz);
}
