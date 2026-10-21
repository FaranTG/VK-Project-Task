using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Topic;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class TopicEndpoints
{
    private const string GetTopicEndpointName = "GetTopic";
    private const string ApiRoute = "/api/topics";

    public static IEndpointRouteBuilder MapTopicEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder commonRouteGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization();

        MapTopicGetEndpoint(commonRouteGroup);
        MapTopicGetByIdEndpoint(commonRouteGroup);

        RouteGroupBuilder organizerRouteGroup = commonRouteGroup
            .MapGroup("/")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapTopicPostEndpoint(organizerRouteGroup);
        MapTopicUpdateEndpoint(organizerRouteGroup);

        return app;
    }

    private static void MapTopicGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (int? pageNumber, int? pageSize, ITopicService topicService) =>
        {
            PaginationDTO? paginationData = (pageNumber.HasValue && pageSize.HasValue)
                ? new PaginationDTO(pageNumber.Value, pageSize.Value)
                : null;

            return Results.Ok(await topicService.GetTopicsAsync(paginationData));
        });
    }

    private static void MapTopicGetByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:int}", async (int id, ITopicService topicService) =>
        {
            QuizApiResponse<TopicInfoDTO> response = await topicService.GetTopicByIdAsync(id);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.NotFound(response);
        })
        .WithName(GetTopicEndpointName);
    }

    private static void MapTopicPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (TopicSaveDTO newTopicData, ITopicService topicService) => 
        {
            QuizApiResponse<TopicInfoDTO> response = await topicService.CreateTopicAsync(newTopicData);

            return response.IsSuccess 
                ? Results.CreatedAtRoute(GetTopicEndpointName, new { id = response.Data!.Id }, response)
                : Results.BadRequest(response); 
        });
    }

    private static void MapTopicUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}", async (int id, TopicSaveDTO newTopicData, ITopicService topicService) =>
        {
            QuizApiResponse response = await topicService.UpdateTopicAsync(id, newTopicData);

            if (response.IsFailure)
            {
                return response.ErrorMessage == ITopicService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}
