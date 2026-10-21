using QuizWebApp.Core.Entities;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<User?> GetByEmailAsync(string email);
    
    Task<bool> ExistsByEmailAsync(string email);
    
    Task AddAsync(User user);

    Task<PagedInfo<User>> GetUsersAsync(PaginationDTO paginationData, bool? isApprovedFilter);

    Task<User?> GetByIdAsync(int id);

    Task UpdateAsync(User user);
}
