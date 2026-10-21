using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Infrastructure.Repositories;

public class TopicsRepository : ITopicsRepository
{
    private readonly QuizContext _dbContext;

    public TopicsRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedInfo<Topic>> GetTopicsAsync(PaginationDTO paginationData)
    {
        IQueryable<Topic> query = _dbContext.Topics
            .AsNoTracking();

        int totalCount = await query
            .CountAsync();

        Topic[] topics = await query
            .OrderBy(topic => topic.Id)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<Topic>(topics, totalCount);
    }

    public Task<Topic?> GetByIdAsync(int id)
        => _dbContext.Topics
            .FirstOrDefaultAsync(topic => topic.Id == id);

    public Task<bool> ExistsByNameAsync(string name)
        => _dbContext.Topics
            .AnyAsync(topic => topic.Name == name);

    public Task<bool> ExistsByNameWithDifferentIdAsync(string name, int id)
        => _dbContext.Topics
            .AnyAsync(topic => topic.Name == name && topic.Id != id);

    public async Task AddAsync(Topic topic)
    {
        _dbContext.Topics
            .Add(topic);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Topic topic)
    {
        _dbContext.Topics
            .Update(topic);

        await _dbContext.SaveChangesAsync();
    }
}
