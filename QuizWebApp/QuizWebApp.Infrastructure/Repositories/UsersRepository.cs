using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly QuizContext _dbContext;

    public UsersRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByEmailAsync(string email)
        => _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email);

    public Task<bool> ExistsByEmailAsync(string email)
        => _dbContext.Users
            .AnyAsync(user => user.Email == email);

    public async Task AddAsync(User user)
    {
        _dbContext.Users
            .Add(user);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedInfo<User>> GetUsersAsync(PaginationDTO paginationData, string? searchFilter, bool? isApprovedFilter)
    {
        IQueryable<User> query = _dbContext.Users
            .AsNoTracking();

        if (isApprovedFilter is not null)
        {
            query = query
                .Where(user => user.IsApproved == isApprovedFilter);
        }

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            string lowerSearch = searchFilter.Trim().ToLower();
            
             query = query
                .Where(user => user.Name.ToLower().Contains(lowerSearch) 
                    || user.Email.ToLower().Contains(lowerSearch)
                    || user.Phone.ToLower().Contains(lowerSearch));
        }

        int totalCount = await query
            .CountAsync();

        User[] users = await query
            .OrderByDescending(user => user.Id)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<User>(users, totalCount);
    }

    public Task<User?> GetByIdAsync(int id)
        => _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == id);

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users
            .Update(user);

        await _dbContext.SaveChangesAsync();
    }
}
