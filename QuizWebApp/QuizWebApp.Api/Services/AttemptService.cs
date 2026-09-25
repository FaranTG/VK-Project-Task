using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.DataEnums;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services;

public class AttemptService : IAttemptService
{
    private const string NoAccessMessage = "You do not have the user rights for this attempt.";

    private readonly QuizContext _dbContext;

    public AttemptService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetActiveQuizzesAsync(int topicIdFilter)
    {
        try
        {
            IQueryable<Quiz> query = _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz => quiz.IsActive);
            
            if (topicIdFilter > 0)
            {
                query = query.Where(quiz => quiz.TopicId == topicIdFilter);
            }
            
            QuizBriefInfoDTO[] quizzes = await query
                .Select
                (
                    quiz => new QuizBriefInfoDTO
                    (
                        quiz.Id,
                        quiz.Name,
                        quiz.TopicId,
                        quiz.Topic!.Name,
                        quiz.QuestionsNumber,
                        quiz.TimeInMinutes
                    )
                )
                .ToArrayAsync();
            
            return QuizApiResponse<QuizBriefInfoDTO[]>.Success(quizzes);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizBriefInfoDTO[]>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<int>> StartQuizAsync(Guid quizId, int participantId)
    {
        try
        {
            Attempt newAttempt = new ()
            {
                ParticipantId = participantId,
                QuizId = quizId,
                Status = nameof(AttemptStatus.Started),
                StartTime = DateTime.UtcNow
            };

            _dbContext.Attempts.Add(newAttempt);
            await _dbContext.SaveChangesAsync();

            return QuizApiResponse<int>.Success(newAttempt.Id);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<int>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<QuestionInfoDTO>> GetNextQuizQuestionAsync(int attemptId, int participantId)
    {
        try
        {
            Attempt? attempt = await _dbContext.Attempts
                .AsNoTracking()
                .Include(attempt => attempt.Quiz)
                    .ThenInclude(quiz => quiz!.Questions)
                        .ThenInclude(question => question.Options)
                .Include(attempt => attempt.AttemptQuestions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(attempt => attempt.Id == attemptId);

            if (attempt is null)
            {
                return QuizApiResponse<QuestionInfoDTO>.Fail(IAttemptService.NotFoundMessage);
            }

            if (attempt.ParticipantId != participantId)
            {
                return QuizApiResponse<QuestionInfoDTO>.Fail(NoAccessMessage);
            }

            int[] usedQuestions = attempt.AttemptQuestions!
                .Select(question => question.QuestionId)
                .ToArray();
            
            QuestionInfoDTO? nextQuestion = attempt.Quiz!.Questions
                .Where(question => !usedQuestions.Contains(question.Id))
                .OrderBy(question => Guid.NewGuid())
                .Select(CreateQuestionInfoDTO)
                .FirstOrDefault();
            
            if (nextQuestion is null)
            {
                return QuizApiResponse<QuestionInfoDTO>.Fail("There are no more questions left for the selected quiz.");
            }

            return QuizApiResponse<QuestionInfoDTO>.Success(nextQuestion);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuestionInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> SaveQuestionResponseAsync(QuestionResponseSaveDTO responseData, int participantId)
    {
        try
        {
            Attempt? attempt = await _dbContext.Attempts
                .Include(attempt => attempt.Quiz)
                    .ThenInclude(quiz => quiz!.Questions)
                        .ThenInclude(question => question.Options)
                .AsSingleQuery()
                .FirstOrDefaultAsync(attempt => attempt.Id == responseData.AttemptId);

            if (attempt is null)
            {
                return QuizApiResponse.Fail(IAttemptService.NotFoundMessage);
            }

            if (attempt.ParticipantId != participantId)
            {
                return QuizApiResponse.Fail(NoAccessMessage);
            }

            Question? question = attempt.Quiz!.Questions
                .FirstOrDefault(question => question.Id == responseData.QuestionId);
            
            if (question is null)
            {
                return QuizApiResponse.Fail("Question not found.");
            }

            AnswerOption? option = question.Options
                .FirstOrDefault(option => option.Id == responseData.SelectedOptionId);
            
            if (option is null)
            {
                return QuizApiResponse.Fail("Selected answer option not found.");
            }

            if (option.IsCorrect)
            {
                ++attempt.Score;
            }

            AttemptQuestion attemptQuestion = new ()
            {
                AttemptId = responseData.AttemptId,
                QuestionId = responseData.QuestionId
            };

            _dbContext.AttemptQuestions.Add(attemptQuestion);

            await _dbContext.SaveChangesAsync();

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> SubmitQuizAsync(int attemptId, AttemptStatus quitStatus, int participantId)
    {
        try
        {
            Attempt? attempt = await _dbContext.Attempts
                .Include(attempt => attempt.AttemptQuestions)
                .FirstOrDefaultAsync(attempt => attempt.Id == attemptId);

            if (attempt is null)
            {
                return QuizApiResponse.Fail(IAttemptService.NotFoundMessage);
            }

            if (attempt.ParticipantId != participantId)
            {
                return QuizApiResponse.Fail(NoAccessMessage);
            }

            if (attempt.Status != nameof(AttemptStatus.Started))
            {
                return QuizApiResponse.Fail("Quiz has already been submitted.");
            }

            attempt.EndTime = DateTime.UtcNow;
            attempt.Status = quitStatus.ToString();

            List<AttemptQuestion> usedQuestions = attempt.AttemptQuestions!.ToList();
            
            _dbContext.AttemptQuestions.RemoveRange(usedQuestions);

            await _dbContext.SaveChangesAsync();

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private QuestionInfoDTO CreateQuestionInfoDTO(Question question)
    {
        return new QuestionInfoDTO
        (
            question.Id,
            question.Text,
            question.Options
                .Select
                (
                    option => new AnswerOptionInfoDTO
                    (
                        option.Id,
                        option.Text,
                        option.IsCorrect
                    )
                )
                .ToList()
        );
    }
}