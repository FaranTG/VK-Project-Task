using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Infrastructure;

public class QuizExpirationWorker : BackgroundService
{
    private const int BatchSize = 100;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuizExpirationWorker> _logger;

    public QuizExpirationWorker(IServiceProvider serviceProvider, ILogger<QuizExpirationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Quiz Expiration Worker started.");

        using var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        
        while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessExpiredAttemptsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while cleaning up expired quiz attempts.");
            }
        }
    }

    /// <summary>
    /// Создает изолированный Scope и координирует этапы обработки просроченных попыток.
    /// </summary>
    private async Task ProcessExpiredAttemptsAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        QuizContext dbContext = scope.ServiceProvider.GetRequiredService<QuizContext>();

        ReadOnlyAttemptMeta[] expiredAttempts = await FetchExpiredAttemptsMetaAsync(dbContext, stoppingToken);

        if (expiredAttempts.Length == 0)
        {
            return;
        }

        _logger.LogWarning("Found {Count} expired quiz attempts. Processing auto-submit...", expiredAttempts.Length);

        await AutoSubmitAttemptsAsync(dbContext, expiredAttempts, stoppingToken);

        int[] attemptIds = expiredAttempts.Select(a => a.Id).ToArray();
        await ClearAttemptQuestionsCacheAsync(dbContext, attemptIds, stoppingToken);
    }

    /// <summary>
    /// Загружает из базы данных ID и вычисленный на стороне СУБД дедлайн для просроченных попыток.
    /// </summary>
    private static async Task<ReadOnlyAttemptMeta[]> FetchExpiredAttemptsMetaAsync(QuizContext dbContext, CancellationToken stoppingToken)
    {
        return await dbContext.Attempts
            .AsNoTracking()
            .Where(attempt => attempt.Status == nameof(AttemptStatus.Started) 
                && attempt.StartTime.AddMinutes(attempt.Quiz!.TimeInMinutes) <= DateTime.UtcNow)
            .OrderBy(attempt => attempt.Id)
            .Select
            (
                attempt => new ReadOnlyAttemptMeta
                (
                    attempt.Id,
                    attempt.StartTime.AddMinutes(attempt.Quiz!.TimeInMinutes)
                )
            )
            .Take(BatchSize)
            .ToArrayAsync(stoppingToken);
    }

    /// <summary>
    /// Перебирает пачку метаданных и обновляет каждую попытку в БД с ее точным индивидуальным временем окончания.
    /// </summary>
    private static async Task AutoSubmitAttemptsAsync(QuizContext dbContext, ReadOnlyAttemptMeta[] expiredAttempts, CancellationToken stoppingToken)
    {
        foreach (ReadOnlyAttemptMeta attemptData in expiredAttempts)
        {
            await dbContext.Attempts
                .Where(attempt => attempt.Id == attemptData.Id)
                .ExecuteUpdateAsync
                (
                    setter => setter
                        .SetProperty(attempt => attempt.Status, nameof(AttemptStatus.AutoSubmitted))
                        .SetProperty(attempt => attempt.EndTime, attemptData.Deadline), 
                    stoppingToken
                );
        }
    }

    /// <summary>
    /// Массово удаляет сессионный кэш вопросов для закрытых попыток.
    /// </summary>
    private async Task ClearAttemptQuestionsCacheAsync(QuizContext dbContext, int[] attemptIds, CancellationToken stoppingToken)
    {
        int deletedQuestionsCacheCount = await dbContext.AttemptQuestions
            .Where(attemptQuestion => attemptIds.Contains(attemptQuestion.AttemptId))
            .ExecuteDeleteAsync(stoppingToken);

        _logger.LogInformation("Successfully auto-submitted expired attempts. Cleaned up {CacheCount} questions from temporary cache.", deletedQuestionsCacheCount);
    }

    private record ReadOnlyAttemptMeta(int Id, DateTime Deadline);
}
