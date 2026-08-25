using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared;
using QuizWebApp.Shared.DTOs.Topic;

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
        MapTopicDeleteEndpoint(organizerRouteGroup);
        MapTopicUpdateEndpoint(organizerRouteGroup);

        return app;
    }

    private static void MapTopicGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (QuizContext dbContext) => 
            Results.Ok(
                await dbContext.Topics
                    .AsNoTracking()
                    .OrderBy(topic => topic.Id)
                    .Select(
                        topic => new TopicInfoDTO
                        (
                            topic.Id,
                            topic.Name
                        )
                    )
                    .ToListAsync()
            )
        );
    }

    private static void MapTopicGetByIdEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:int}", async (int id, QuizContext dbContext) =>
        {
            Topic? topic = await dbContext.Topics
                .AsNoTracking()
                .FirstOrDefaultAsync(topic => topic.Id == id);

            return topic is null ?
                Results.NotFound()
                : Results.Ok(
                    new TopicInfoDTO
                    (
                        topic.Id,
                        topic.Name
                    )
                );
        })
        .WithName(GetTopicEndpointName);
    }

    private static void MapTopicPostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (TopicSaveDTO newTopicData, QuizContext dbContext) => 
        {
            Topic topic = new ()
            {
                Name = newTopicData.Name
            };

            dbContext.Topics.Add(topic);
            await dbContext.SaveChangesAsync();

            TopicInfoDTO createdTopicData = new
            (
                topic.Id,
                topic.Name
            );

            return Results.CreatedAtRoute(GetTopicEndpointName, new { id = createdTopicData.Id }, createdTopicData);
        });
    }

    private static void MapTopicUpdateEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}", async (int id, TopicSaveDTO newTopicData, QuizContext dbContext) =>
        {
            Topic? topic = await dbContext.Topics.FindAsync(id);

            if (topic is null)
            {
                return Results.NotFound();
            }

            topic.Name = newTopicData.Name;
            
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    private static void MapTopicDeleteEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:int}", async (int id, QuizContext dbContext) =>
        {
            await dbContext.Topics
                .Where(topic => topic.Id == id)
                .ExecuteDeleteAsync();
                
            return Results.NoContent();
        });
    }
}
