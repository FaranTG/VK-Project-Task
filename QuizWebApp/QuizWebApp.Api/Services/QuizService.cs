using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Api.Validation;
using QuizWebApp.Core.Validation;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Services;

public class QuizService : IQuizService
{
    private const string DuplicateMessage = "A quiz with this name already exists.";

    private readonly QuizContext _dbContext;

    public QuizService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(PaginationDTO paginationData, int topicFilterId)
    {
        try
        {
            IQueryable<Quiz> query = _dbContext.Quizzes.AsNoTracking();

            if (topicFilterId > 0)
            {
                query = query.Where(quiz => quiz.TopicId == topicFilterId);
            }

            int totalCount = await query.CountAsync();
            QuizSummaryInfoDTO[] quizzes = await query
                .OrderBy(quiz => quiz.Id)
                .Paginate(paginationData)
                .Select
                (
                    quiz => new QuizSummaryInfoDTO
                    (
                        quiz.Id,
                        quiz.Name,
                        quiz.TopicId,
                        quiz.Topic!.Name,
                        quiz.QuestionsNumber,
                        quiz.TimeInMinutes,
                        quiz.IsActive,
                        quiz.Questions
                            .Select(question => question.Text)
                            .ToList()
                    )
                )
                .ToArrayAsync();
            
            PagedInfo<QuizSummaryInfoDTO> pagedQuizzes = new (quizzes, totalCount);
            
            return QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>.Success(pagedQuizzes);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>> GetAvailableQuizzesAsync(PaginationDTO paginationData, int topicFilterId)
    {
        try
        {
            IQueryable<Quiz> query = _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz => quiz.IsActive);

            if (topicFilterId > 0)
            {
                query = query.Where(quiz => quiz.TopicId == topicFilterId);
            }
            
            int totalCount = await query.CountAsync();
            QuizBriefInfoDTO[] quizzes = await query
                .OrderBy(quiz => quiz.Id)
                .Paginate(paginationData)
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
            
            PagedInfo<QuizBriefInfoDTO> pagedQuizzes = new (quizzes, totalCount);
            
            return QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>.Success(pagedQuizzes);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<QuizInfoDTO>> GetQuizByIdAsync(Guid id)
    {
        try
        {
            Quiz? quiz = await _dbContext.Quizzes
                .AsNoTracking()
                .Include(quiz => quiz.Questions)
                    .ThenInclude(question => question.Options)
                .AsSplitQuery()
                .FirstOrDefaultAsync(quiz => quiz.Id == id);
            
            return quiz is null
                ? QuizApiResponse<QuizInfoDTO>.Fail(IQuizService.NotFoundMessage)
                : QuizApiResponse<QuizInfoDTO>.Success(CreateQuizInfoDTO(quiz));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<QuizTitleInfoDTO>> GetQuizTitleByIdAsync(Guid id)
    {
        try
        {
            QuizTitleInfoDTO? quizTitle = await _dbContext.Quizzes
                .AsNoTracking()
                .Where(quiz => quiz.Id == id) 
                .Select
                (
                    quiz => new QuizTitleInfoDTO
                    (
                        quiz.Id,
                        quiz.Name,
                        quiz.Topic!.Name
                    )
                )
                .SingleOrDefaultAsync(); 
            
            return quizTitle is null
                ? QuizApiResponse<QuizTitleInfoDTO>.Fail(IQuizService.NotFoundMessage)
                : QuizApiResponse<QuizTitleInfoDTO>.Success(quizTitle);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizTitleInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<QuizInfoDTO>> CreateQuizAsync(QuizSaveDTO newQuizData)
    {
        try
        {
            string? validationError = newQuizData.Validate();
            if (validationError is not null)
            {
                return QuizApiResponse<QuizInfoDTO>.Fail(validationError);
            }

            if (await _dbContext.Quizzes.AnyAsync(quiz => quiz.Name == newQuizData.Name))
            {
                return QuizApiResponse<QuizInfoDTO>.Fail(DuplicateMessage);
            }

            Quiz quiz = new ()
            {
                Name = newQuizData.Name,
                TopicId = newQuizData.TopicId,
                QuestionsNumber = newQuizData.Questions.Count,
                TimeInMinutes = newQuizData.TimeInMinutes,
                IsActive = newQuizData.IsActive,
                Questions = CreateQuestionList(newQuizData.Questions)
            };

            _dbContext.Quizzes.Add(quiz);
            await _dbContext.SaveChangesAsync();

            QuizInfoDTO createdQuizData = CreateQuizInfoDTO(quiz);
            return QuizApiResponse<QuizInfoDTO>.Success(createdQuizData);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> UpdateQuizAsync(Guid id, QuizSaveDTO newQuizData)
    {
        try
        {
            string? validationError = newQuizData.Validate();
            if (validationError is not null)
            {
                return QuizApiResponse.Fail(validationError);
            }

            Quiz? quiz = await _dbContext.Quizzes.FindAsync(id);

            if (quiz is null)
            {
                return QuizApiResponse.Fail(IQuizService.NotFoundMessage);
            }

            if (await _dbContext.Quizzes.AnyAsync(quiz => quiz.Name == newQuizData.Name && quiz.Id != id))
            {
                return QuizApiResponse.Fail(DuplicateMessage);
            }

            await _dbContext.Questions
                .Where(question => question.QuizId == id)
                .ExecuteDeleteAsync();

            quiz.Name = newQuizData.Name;
            quiz.TopicId = newQuizData.TopicId;
            quiz.QuestionsNumber = newQuizData.Questions.Count;
            quiz.TimeInMinutes = newQuizData.TimeInMinutes;
            quiz.IsActive = newQuizData.IsActive;
            quiz.Questions = CreateQuestionList(newQuizData.Questions);
            
            await _dbContext.SaveChangesAsync();

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private QuizInfoDTO CreateQuizInfoDTO(Quiz quiz)
    {
        return new
        (
            quiz.Id,
            quiz.Name,
            quiz.TopicId,
            quiz.QuestionsNumber,
            quiz.TimeInMinutes,
            quiz.IsActive,
            CreateQuestionInfoDTOList(quiz.Questions)
        );
    }

    private List<QuestionInfoDTO> CreateQuestionInfoDTOList(ICollection<Question> questions)
    {
        return questions
            .Select
            (
                question => new QuestionInfoDTO
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
                )
            )
            .ToList();
    }

    private ICollection<Question> CreateQuestionList(List<QuestionSaveDTO> saveDTOList)
    {
        return saveDTOList
            .Select
            (
                questionSaveDTO => new Question
                {
                    Text = questionSaveDTO.Text,
                    Options = questionSaveDTO.Options
                        .Select
                        (
                            optionSaveDTO => new AnswerOption
                            {
                                Text = optionSaveDTO.Text,
                                IsCorrect = optionSaveDTO.IsCorrect
                            }
                        )
                        .ToList()
                }
            )
            .ToList();
    }
}
