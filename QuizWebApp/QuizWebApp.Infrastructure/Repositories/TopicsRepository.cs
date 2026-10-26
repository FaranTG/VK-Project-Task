using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Infrastructure.Repositories;

public class TopicsRepository : ITopicsRepository
{
    private readonly QuizContext _dbContext;

    public TopicsRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedInfo<Topic>> GetTopicsAsync(PaginationDTO paginationData, TopicFilterDTO filterData)
    {
        IQueryable<Topic> query = _dbContext.Topics
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            string lowerSearch = filterData.SearchFilter.Trim().ToLower();
            
            query = query
                .Where(quiz => quiz.Name.ToLower().Contains(lowerSearch));
        }

        int totalCount = await query
            .CountAsync();

        Topic[] topics = await query
            .OrderBy(topic => topic.Id)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<Topic>(topics, totalCount);
    }

    public async Task<PagedInfo<Topic>> GetFilteredQuizzesTopicsAsync(PaginationDTO paginationData, QuizFilterDTO filterData)
    {
         IQueryable<Quiz> query = _dbContext.Quizzes
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            string lowerSearch = filterData.SearchFilter.Trim().ToLower();
            
            query = query
                .Where(quiz => quiz.Name.ToLower().Contains(lowerSearch));
        }

        IQueryable<Topic> topicQuery = query
            .Select(quiz => quiz.Topic!)
            .Distinct();

        int totalCount = await topicQuery.CountAsync();

        Topic[] topics = await topicQuery
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
