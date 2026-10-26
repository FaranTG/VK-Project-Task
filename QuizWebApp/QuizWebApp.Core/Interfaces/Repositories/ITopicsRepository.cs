using QuizWebApp.Core.Entities;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface ITopicsRepository
{
    Task<PagedInfo<Topic>> GetTopicsAsync(PaginationDTO paginationData, TopicFilterDTO filterData);

    Task<PagedInfo<Topic>> GetFilteredQuizzesTopicsAsync(PaginationDTO paginationData, QuizFilterDTO filterData);
    
    Task<Topic?> GetByIdAsync(int id);
    
    Task<bool> ExistsByNameAsync(string name);
    
    Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id);
    
    Task AddAsync(Topic topic);
    
    Task UpdateAsync(Topic topic);
}
