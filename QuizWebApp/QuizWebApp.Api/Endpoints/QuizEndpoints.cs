using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Quiz;
using QuizWebApp.Shared.Enums;

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
        MapQuizUpdateEndpoint(organizerRouteGroup);

        return app;
    }

    private static void MapQuizGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (IQuizService quizService) =>
            Results.Ok(await quizService.GetQuizzesAsync())
        );
    }

    private static void MapQuizGetByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (Guid id, IQuizService quizService) =>
        {
            QuizApiResponse<QuizInfoDTO> response = await quizService.GetQuizByIdAsync(id);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithName(GetQuizEndpointName);
    }

    private static void MapQuizPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (QuizSaveDTO newQuizData, IQuizService quizService) => 
        {
            QuizApiResponse<QuizInfoDTO> response = await quizService.CreateQuizAsync(newQuizData);

            return response.IsSuccess 
                ? Results.CreatedAtRoute(GetQuizEndpointName, new { id = response.Data!.Id }, response)
                : Results.BadRequest(response);
        });
    }

    private static void MapQuizUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (Guid id, QuizSaveDTO newQuizData, IQuizService quizService) =>
        {
            QuizApiResponse response = await quizService.UpdateQuizAsync(id, newQuizData);
            
            if (response.IsFailure)
            {
                return response.ErrorMessage == IQuizService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}