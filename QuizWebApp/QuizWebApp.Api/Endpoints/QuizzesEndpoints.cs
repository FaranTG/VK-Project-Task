using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.Quiz;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class QuizzesEndpoints
{
    private const string GetQuizEndpointName = "GetQuiz";
    private const string GetQuizTitleEndpointName = "GetQuizTitle";
    private const string ApiRoute = "/api/quizzes";

    public static IEndpointRouteBuilder MapQuizzesEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder commonRouteGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization();

        MapCommonEndpoints(commonRouteGroup);
        MapOrganizerEndpoints(commonRouteGroup);

        return app;
    }

    private static void MapCommonEndpoints(IEndpointRouteBuilder app)
    {
        MapQuizGetAvailableEndpoint(app);
    }

    private static void MapOrganizerEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder organizerRouteGroup = app
            .MapGroup("/")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapQuizGetEndpoint(organizerRouteGroup);
        MapQuizPostEndpoint(organizerRouteGroup);
        MapQuizUpdateEndpoint(organizerRouteGroup);
        MapQuizGetByIdEndpoint(organizerRouteGroup);
        MapQuizGetTitleByIdEndpoint(organizerRouteGroup);
    }

    private static void MapQuizGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([AsParameters] PaginationDTO paginationData, [AsParameters] QuizFilterDTO filterData, IQuizzesService quizService) =>
            Results.Ok(await quizService.GetQuizzesAsync(paginationData, filterData))
        );
    }

    private static void MapQuizGetAvailableEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/available", async ([AsParameters] PaginationDTO paginationData, [AsParameters] QuizFilterDTO filterData, IQuizzesService quizService) =>
            Results.Ok(await quizService.GetAvailableQuizzesAsync(paginationData, filterData))
        );
    }

    private static void MapQuizGetByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (Guid id, IQuizzesService quizService) =>
        {
            QuizApiResponse<QuizInfoDTO> response = await quizService.GetQuizByIdAsync(id);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithName(GetQuizEndpointName);
    }

    private static void MapQuizGetTitleByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/title", async (Guid id, IQuizzesService quizService) =>
        {
            QuizApiResponse<QuizTitleInfoDTO> response = await quizService.GetQuizTitleByIdAsync(id);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithName(GetQuizTitleEndpointName);
    }

    private static void MapQuizPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (QuizSaveDTO newQuizData, IQuizzesService quizService) => 
        {
            QuizApiResponse<QuizInfoDTO> response = await quizService.CreateQuizAsync(newQuizData);

            return response.IsSuccess 
                ? Results.CreatedAtRoute(GetQuizEndpointName, new { id = response.Data!.Id }, response)
                : Results.BadRequest(response);
        });
    }

    private static void MapQuizUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (Guid id, QuizSaveDTO newQuizData, IQuizzesService quizService) =>
        {
            QuizApiResponse response = await quizService.UpdateQuizAsync(id, newQuizData);
            
            if (response.IsFailure)
            {
                return response.ErrorMessage == IQuizzesService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}