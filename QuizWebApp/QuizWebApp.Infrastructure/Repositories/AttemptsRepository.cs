using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Infrastructure.Repositories;

public class AttemptsRepository : IAttemptsRepository
{
    private readonly QuizContext _dbContext;

    public AttemptsRepository(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Attempt?> GetByIdWithQuizAndQuestionsAsync(int id)
        => _dbContext.Attempts
            .AsNoTracking()
            .Include(attempt => attempt.Quiz)
                .ThenInclude(quiz => quiz!.Questions)
                    .ThenInclude(question => question.Options)
            .Include(attempt => attempt.AttemptQuestions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(attempt => attempt.Id == id);

    public Task<Attempt?> GetByIdWithQuestionsOnlyAsync(int id)
        => _dbContext.Attempts
            .Include(attempt => attempt.AttemptQuestions)
            .FirstOrDefaultAsync(attempt => attempt.Id == id);

    public async Task AddAsync(Attempt attempt)
    {
        _dbContext.Attempts
            .Add(attempt);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Attempt attempt)
    {
        _dbContext.Attempts
            .Update(attempt);

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAttemptQuestionAsync(AttemptQuestion attemptQuestion)
    {
        _dbContext.AttemptQuestions
            .Add(attemptQuestion);

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAttemptQuestionsRangeAsync(IEnumerable<AttemptQuestion> attemptQuestions)
    {
        _dbContext.AttemptQuestions
            .RemoveRange(attemptQuestions);

        await _dbContext.SaveChangesAsync();
    }

    public Task<PagedInfo<Attempt>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData, AttemptFilterDTO filterData)
        => CoreGetAttemptsAsync
        (
            query => PrepareQuizAttemptsQuery(query, quizId, filterData),
            paginationData,
            filterData
        );

    public Task<PagedInfo<Attempt>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData, AttemptFilterDTO filterData)
        => CoreGetAttemptsAsync
        (
            query => PrepareParticipantAttemptsQuery(query, participantId, filterData),
            paginationData,
            filterData
        );
    
    private IQueryable<Attempt> PrepareQuizAttemptsQuery(IQueryable<Attempt> query, Guid quizId, AttemptFilterDTO filterData)
    {
        query = query
            .Where(attempt => attempt.QuizId == quizId)
            .Include(attempt => attempt.Participant);

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            string lowerSearch = filterData.SearchFilter.Trim().ToLower();
            query = query.Where(attempt => attempt.Participant!.Name.ToLower().Contains(lowerSearch) 
                || attempt.Participant.Email.ToLower().Contains(lowerSearch));
        }

        return query;
    }

    private IQueryable<Attempt> PrepareParticipantAttemptsQuery(IQueryable<Attempt> query, int participantId, AttemptFilterDTO filterData)
    {
        query = query
            .Where(attempt => attempt.ParticipantId == participantId)
            .Include(attempt => attempt.Quiz)     
                .ThenInclude(quiz => quiz!.Topic)
            .AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            string lowerSearch = filterData.SearchFilter.Trim().ToLower();
            query = query.Where(attempt => attempt.Quiz!.Name.ToLower().Contains(lowerSearch) 
                || attempt.Quiz.Topic!.Name.ToLower().Contains(lowerSearch));
        }

        return query;
    }

    private async Task<PagedInfo<Attempt>> CoreGetAttemptsAsync
    (
        Func<IQueryable<Attempt>, IQueryable<Attempt>> queryBuilder,
        PaginationDTO paginationData,
        AttemptFilterDTO filterData
    )
    {
        IQueryable<Attempt> query = _dbContext.Attempts
            .AsNoTracking();

        query = queryBuilder(query);

        if (filterData.StatusFilter is not null)
        {
            query = query
                .Where(attempt => attempt.Status == filterData.StatusFilter.ToString());
        }

        int totalCount = await query
            .CountAsync();

        Attempt[] attempts = await query    
            .OrderByDescending(attempt => attempt.StartTime)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<Attempt>(attempts, totalCount);
    }
}
