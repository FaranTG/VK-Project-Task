using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Infrastructure.Repositories;

public class QuizzesRepository : IQuizzesRepository
{
    private readonly QuizContext _dbContext;

    public QuizzesRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Quiz?> GetByIdWithQuestionsAndOptionsAsync(Guid id)
        => _dbContext.Quizzes
            .AsNoTracking()
            .Include(quiz => quiz.Questions)
                .ThenInclude(question => question.Options)
            .AsSplitQuery()
            .FirstOrDefaultAsync(quiz => quiz.Id == id);

    public Task<Quiz?> GetByIdAsync(Guid id)
        => _dbContext.Quizzes
            .Include(quiz => quiz.Topic)
            .FirstOrDefaultAsync(quiz => quiz.Id == id);

    public Task<bool> ExistsByNameAsync(string name)
        => _dbContext.Quizzes
            .AnyAsync(quiz => quiz.Name == name);

    public Task<bool> ExistsByNameWithDifferentIdAsync(string name, Guid id)
        => _dbContext.Quizzes
            .AnyAsync(quiz => quiz.Name == name && quiz.Id != id);

    public Task DeleteQuestionsByQuizIdAsync(Guid quizId)
        => _dbContext.Questions
            .Where(question => question.QuizId == quizId)
            .ExecuteDeleteAsync();

    public async Task AddAsync(Quiz quiz)
    {
        _dbContext.Quizzes
            .Add(quiz);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Quiz quiz)
    {
        _dbContext.Quizzes
            .Update(quiz);

        await _dbContext.SaveChangesAsync();
    }

    public Task<PagedInfo<Quiz>> GetQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData)
        => CoreGetQuizzesAsync(paginationData, filterData, null);
    
    public Task<PagedInfo<Quiz>> GetAvailableQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData)
        => CoreGetQuizzesAsync(paginationData, filterData, true);

    private async Task<PagedInfo<Quiz>> CoreGetQuizzesAsync(PaginationDTO paginationData, QuizFilterDTO filterData, bool? isActiveFilter)
    {
        IQueryable<Quiz> query = _dbContext.Quizzes
            .AsNoTracking();

        if (isActiveFilter is not null)
        {
            query = query
                .Where(quiz => quiz.IsActive == isActiveFilter);
        }

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            string lowerSearch = filterData.SearchFilter.Trim().ToLower();
            
            query = query
                .Where(quiz => quiz.Name.ToLower().Contains(lowerSearch));
        }

        if (filterData.TopicFilterId > 0)
        {
            query = query
                .Where(quiz => quiz.TopicId == filterData.TopicFilterId);
        }

        int totalCount = await query
            .CountAsync();

        Quiz[] quizzes = await query
            .Include(quiz => quiz.Topic)
            .Include(quiz => quiz.Questions)
            .AsSplitQuery()
            .OrderBy(quiz => quiz.Id)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<Quiz>(quizzes, totalCount);
    }
}
