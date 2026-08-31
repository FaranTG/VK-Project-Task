using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Api.Validation;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.AnswerOption;
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

    public async Task<QuizApiResponse<QuizBriefInfoDTO[]>> GetQuizzesAsync()
    {
        try
        {
            QuizBriefInfoDTO[] quizzes = await _dbContext.Quizzes
                .AsNoTracking()
                .Select
                (
                    quiz => new QuizBriefInfoDTO
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
            
            return QuizApiResponse<QuizBriefInfoDTO[]>.Success(quizzes);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<QuizBriefInfoDTO[]>.Fail(exception.Message);
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
