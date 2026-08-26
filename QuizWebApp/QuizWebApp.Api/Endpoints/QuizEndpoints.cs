using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared;
using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Endpoints;

public static class QuizEndpoints
{
    private const string GetQuizEndpointName = "GetQuiz";
    private const string ApiRoute = "/api/quizzes";

    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder commonRouteGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization();

        MapQuizGetEndpoint(commonRouteGroup);
        MapQuizGetByIdEndpoint(commonRouteGroup);

        RouteGroupBuilder organizerRouteGroup = commonRouteGroup
            .MapGroup("/")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapQuizPostEndpoint(organizerRouteGroup);
        /*
        MapQuizDeleteEndpoint(organizerRouteGroup);
        */
        MapQuizUpdateEndpoint(organizerRouteGroup);

        return app;
    }

    private static void MapQuizGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (QuizContext dbContext) =>
            await dbContext.Quizzes
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
                        quiz.IsActive
                    )
                )
                .ToListAsync()
        );
    }

    private static void MapQuizGetByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (Guid id, QuizContext dbContext) =>
        {
            Quiz? quiz = await dbContext.Quizzes
                .AsNoTracking()
                .Include(quiz => quiz.Questions)
                    .ThenInclude(question => question.Options)
                .AsSplitQuery()
                .FirstOrDefaultAsync(quiz => quiz.Id == id);

            return quiz is null ?
                Results.NotFound()
                : Results.Ok(
                    CreateQuizInfoDTO(quiz)
                );
        })
        .WithName(GetQuizEndpointName);
    }

    private static void MapQuizPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (QuizSaveDTO newQuizDTO, QuizContext dbContext) => 
        {
            Quiz quiz = new ()
            {
                Name = newQuizDTO.Name,
                TopicId = newQuizDTO.TopicId,
                QuestionsNumber = newQuizDTO.Questions.Count,
                TimeInMinutes = newQuizDTO.TimeInMinutes,
                IsActive = newQuizDTO.IsActive,
                Questions = CreateQuestionList(newQuizDTO.Questions)
            };

            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync();

            QuizInfoDTO createdQuizInfoDTO = CreateQuizInfoDTO(quiz);

            return Results.CreatedAtRoute(GetQuizEndpointName, new { id = createdQuizInfoDTO.Id }, createdQuizInfoDTO);
        });
    }

    private static void MapQuizUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (Guid id, QuizSaveDTO newQuizDTO, QuizContext dbContext) =>
        {
            Quiz? quiz = await dbContext.Quizzes.FindAsync(id);

            if (quiz is null)
            {
                return Results.NotFound();
            }

            await dbContext.Questions
                .Where(question => question.QuizId == id)
                .ExecuteDeleteAsync();
            
            quiz.Name = newQuizDTO.Name;
            quiz.TopicId = newQuizDTO.TopicId;
            quiz.QuestionsNumber = newQuizDTO.Questions.Count;
            quiz.TimeInMinutes = newQuizDTO.TimeInMinutes;
            quiz.IsActive = newQuizDTO.IsActive;
            quiz.Questions = CreateQuestionList(newQuizDTO.Questions);
            
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    private static void MapQuizDeleteEndpoint(IEndpointRouteBuilder app)
    {
        throw new NotImplementedException();
    }

    private static ICollection<Question> CreateQuestionList(List<QuestionSaveDTO> saveDTOList)
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

    private static QuizInfoDTO CreateQuizInfoDTO(Quiz quiz)
    {
        return new
        (
            quiz.Id,
            quiz.Name,
            quiz.TopicId,
            quiz.QuestionsNumber,
            quiz.TimeInMinutes,
            quiz.IsActive,
            quiz.Questions
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
                .ToList()
        );
    }
}