using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.Topic;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class TopicsEndpoints
{
    private const string ApiRoute = "/api/topics";

    public static IEndpointRouteBuilder MapTopicsEndpoints(this IEndpointRouteBuilder app)
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
        MapTopicGetEndpoint(app);
        MapGetFilteredQuizzesTopicsEndpoint(app);
    }

    private static void MapOrganizerEndpoints(IEndpointRouteBuilder app)
    {
       RouteGroupBuilder organizerRouteGroup = app
            .MapGroup("/")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapTopicPostEndpoint(organizerRouteGroup);
        MapTopicUpdateEndpoint(organizerRouteGroup);
    }

    private static void MapTopicGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (int? pageNumber, int? pageSize, [AsParameters] TopicFilterDTO filterData, ITopicsService topicService) =>
        {
            PaginationDTO? paginationData = (pageNumber.HasValue && pageSize.HasValue)
                ? new PaginationDTO(pageNumber.Value, pageSize.Value)
                : null;

            return Results.Ok(await topicService.GetTopicsAsync(paginationData, filterData));
        });
    }

    private static void MapGetFilteredQuizzesTopicsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/quizzes-usage", async (int? pageNumber, int? pageSize, [AsParameters] QuizFilterDTO filterData, ITopicsService topicService) =>
        {
            PaginationDTO? paginationData = (pageNumber.HasValue && pageSize.HasValue)
                ? new PaginationDTO(pageNumber.Value, pageSize.Value)
                : null;

            return Results.Ok(await topicService.GetFilteredQuizzesTopicsAsync(paginationData, filterData));
        });
    }

    private static void MapTopicPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (TopicSaveDTO newTopicData, ITopicsService topicService) => 
        {
            QuizApiResponse<TopicInfoDTO> response = await topicService.CreateTopicAsync(newTopicData);

            return response.IsSuccess 
                ? Results.Json(response, statusCode: 201)
                : Results.BadRequest(response); 
        });
    }

    private static void MapTopicUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}", async (int id, TopicSaveDTO newTopicData, ITopicsService topicService) =>
        {
            QuizApiResponse response = await topicService.UpdateTopicAsync(id, newTopicData);

            if (response.IsFailure)
            {
                return response.ErrorMessage == ITopicsService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}
