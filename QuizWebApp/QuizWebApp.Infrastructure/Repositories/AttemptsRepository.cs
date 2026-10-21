using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;

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

    public Task<PagedInfo<Attempt>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData)
        => CoreGetAttemptsAsync(attempt => attempt.QuizId == quizId, paginationData);

    public Task<PagedInfo<Attempt>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData)
        => CoreGetAttemptsAsync(attempt => attempt.ParticipantId == participantId, paginationData);

    private async Task<PagedInfo<Attempt>> CoreGetAttemptsAsync
    (
        Expression<Func<Attempt, bool>> predicate,
        PaginationDTO paginationData
    )
    {
        IQueryable<Attempt> query = _dbContext.Attempts
            .AsNoTracking()
            .Where(predicate);

        int totalCount = await query
            .CountAsync();

        Attempt[] attempts = await query
            .OrderByDescending(attempt => attempt.StartTime)
            .Paginate(paginationData)
            .ToArrayAsync();

        return new PagedInfo<Attempt>(attempts, totalCount);
    }
}
