using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Attempt;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Core.UseCases;

public class AttemptService : IAttemptService
{
    private const string NoAccessMessage = "You do not have the user rights for this attempt.";

    private readonly IAttemptsRepository _attemptsRepository;

    public AttemptService(IAttemptsRepository attemptsRepository)
    {
        _attemptsRepository = attemptsRepository;
    }

    public async Task<QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData)
    {
        try
        {
            PagedInfo<Attempt> pagedAttempts = await _attemptsRepository
                .GetQuizAttemptsAsync(quizId, paginationData);

            AttemptParticipantInfoDTO[] participants = pagedAttempts.DTOs
                .Select(CreateAttemptParticipantInfoDTO)
                .ToArray();

            PagedInfo<AttemptParticipantInfoDTO> result = new (participants, pagedAttempts.TotalCount);

            return QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>.Success(result);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<AttemptParticipantInfoDTO>>.Fail(exception.Message);
        }
    }
    
    public async Task<QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData)
    {
        try
        {
            PagedInfo<Attempt> pagedAttempts = await _attemptsRepository
                .GetParticipantAttemptsAsync(participantId, paginationData);

            AttemptQuizInfoDTO[] attempts = pagedAttempts.DTOs
                .Select(CreateAttemptQuizInfoDTO)
                .ToArray();
            
            PagedInfo<AttemptQuizInfoDTO> result = new (attempts, pagedAttempts.TotalCount);
            
            return QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>.Success(result);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<AttemptQuizInfoDTO>>.Fail(exception.Message);
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

            await _attemptsRepository
                .AddAsync(newAttempt);

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
            Attempt? attempt = await _attemptsRepository
                .GetByIdWithQuizAndQuestionsAsync(attemptId);

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
                .OrderBy(_ => Guid.NewGuid())
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
            Attempt? attempt = await _attemptsRepository
                .GetByIdWithQuizAndQuestionsAsync(responseData.AttemptId);

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

            await _attemptsRepository
                .AddAttemptQuestionAsync(attemptQuestion);

            await _attemptsRepository
                .UpdateAsync(attempt);

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
            Attempt? attempt = await _attemptsRepository
                .GetByIdWithQuestionsOnlyAsync(attemptId);

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
            
            await _attemptsRepository
                .RemoveAttemptQuestionsRangeAsync(usedQuestions);

            await _attemptsRepository
                .UpdateAsync(attempt);

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private AttemptParticipantInfoDTO CreateAttemptParticipantInfoDTO(Attempt attempt)
        => new 
        (
            attempt.Id,
            attempt.ParticipantId,
            attempt.Participant!.Name,
            attempt.Participant.Email,
            attempt.Status,
            attempt.StartTime,
            attempt.EndTime,
            attempt.Score
        );

    private AttemptQuizInfoDTO CreateAttemptQuizInfoDTO(Attempt attempt)
        => new 
        (
            attempt.Id,
            attempt.QuizId,
            attempt.Quiz!.Name,
            attempt.Quiz.Topic!.Name,
            attempt.Status,
            attempt.StartTime,
            attempt.EndTime,
            attempt.Score
        );

    private QuestionInfoDTO CreateQuestionInfoDTO(Question question)
        => new 
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
