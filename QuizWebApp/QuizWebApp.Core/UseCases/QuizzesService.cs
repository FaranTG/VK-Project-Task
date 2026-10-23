using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Core.Validation;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Core.UseCases;

public class QuizzesService : IQuizzesService
{
    private const string DuplicateMessage = "A quiz with this name already exists.";

    private readonly IQuizzesRepository _quizzesRepository;

    public QuizzesService(IQuizzesRepository quizzesRepository)
    {
        _quizzesRepository = quizzesRepository;
    }

    public async Task<QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>> GetQuizzesAsync(PaginationDTO paginationData, int topicFilterId)
    {
        try
        {
            PagedInfo<Quiz> pagedQuizzes = await _quizzesRepository
                .GetQuizzesAsync(paginationData, topicFilterId);

            QuizSummaryInfoDTO[] quizzes = pagedQuizzes.Data
                .Select(CreateQuizSummaryInfoDTO)
                .ToArray();
            
            PagedInfo<QuizSummaryInfoDTO> result = new (quizzes, pagedQuizzes.TotalCount);
            
            return QuizApiResponse<PagedInfo<QuizSummaryInfoDTO>>.Success(result);
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
            PagedInfo<Quiz> pagedQuizzes = await _quizzesRepository
                .GetAvailableQuizzesAsync(paginationData, topicFilterId);

            QuizBriefInfoDTO[] quizzes = pagedQuizzes.Data
                .Select(CreateQuizBriefInfoDTO)
                .ToArray();
            
            PagedInfo<QuizBriefInfoDTO> result = new (quizzes, pagedQuizzes.TotalCount);
            
            return QuizApiResponse<PagedInfo<QuizBriefInfoDTO>>.Success(result);
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
            Quiz? quiz = await _quizzesRepository
                .GetByIdWithQuestionsAndOptionsAsync(id);
            
            return quiz is null
                ? QuizApiResponse<QuizInfoDTO>.Fail(IQuizzesService.NotFoundMessage)
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
            Quiz? quiz = await _quizzesRepository
                .GetByIdAsync(id);

            if (quiz is null)
            {
                return QuizApiResponse<QuizTitleInfoDTO>.Fail(IQuizzesService.NotFoundMessage);
            }
            
            QuizTitleInfoDTO quizTitle = new (quiz.Id, quiz.Name, quiz.Topic!.Name);
            
            return QuizApiResponse<QuizTitleInfoDTO>.Success(quizTitle);
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

            bool isQuizDuplicate = await _quizzesRepository
                .ExistsByNameAsync(newQuizData.Name);

            if (isQuizDuplicate)
            {
                return QuizApiResponse<QuizInfoDTO>.Fail(DuplicateMessage);
            }

            Quiz quiz = CreateQuizModel(newQuizData);

            await _quizzesRepository
                .AddAsync(quiz);

            return QuizApiResponse<QuizInfoDTO>.Success(CreateQuizInfoDTO(quiz));
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

            Quiz? quiz = await _quizzesRepository
                .GetByIdAsync(id);

            if (quiz is null)
            {
                return QuizApiResponse.Fail(IQuizzesService.NotFoundMessage);
            }

            bool isNameDuplicate = await _quizzesRepository
                .ExistsByNameWithDifferentIdAsync(newQuizData.Name, id);

            if (isNameDuplicate)
            {
                return QuizApiResponse.Fail(DuplicateMessage);
            }

            await _quizzesRepository
                .DeleteQuestionsByQuizIdAsync(id);
            
            await _quizzesRepository
                .UpdateAsync(UpdateQuizModel(quiz, newQuizData));

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private Quiz UpdateQuizModel(Quiz quiz, QuizSaveDTO newQuizData)
    {
        quiz.Name = newQuizData.Name;
        quiz.TopicId = newQuizData.TopicId;
        quiz.QuestionsNumber = newQuizData.Questions.Count;
        quiz.TimeInMinutes = newQuizData.TimeInMinutes;
        quiz.IsActive = newQuizData.IsActive;
        quiz.Questions = CreateQuestionModelsList(newQuizData.Questions);

        return quiz;
    }

    private Quiz CreateQuizModel(QuizSaveDTO newQuizData)
        => new ()
            {
                Name = newQuizData.Name,
                TopicId = newQuizData.TopicId,
                QuestionsNumber = newQuizData.Questions.Count,
                TimeInMinutes = newQuizData.TimeInMinutes,
                IsActive = newQuizData.IsActive,
                Questions = CreateQuestionModelsList(newQuizData.Questions)
            };

    private QuizSummaryInfoDTO CreateQuizSummaryInfoDTO(Quiz quiz)
        => new 
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
        );

    private QuizBriefInfoDTO CreateQuizBriefInfoDTO(Quiz quiz)
        => new 
        (
            quiz.Id,
            quiz.Name,
            quiz.TopicId,
            quiz.Topic!.Name,
            quiz.QuestionsNumber,
            quiz.TimeInMinutes
        );

    private QuizInfoDTO CreateQuizInfoDTO(Quiz quiz)
        => new 
        (
            quiz.Id,
            quiz.Name,
            quiz.TopicId,
            quiz.QuestionsNumber,
            quiz.TimeInMinutes,
            quiz.IsActive,
            CreateQuestionInfoDTOList(quiz.Questions)
        );

    private List<QuestionInfoDTO> CreateQuestionInfoDTOList(List<Question> questions)
        => questions
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

    private List<Question> CreateQuestionModelsList(List<QuestionSaveDTO> saveDTOList)
        => saveDTOList
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
