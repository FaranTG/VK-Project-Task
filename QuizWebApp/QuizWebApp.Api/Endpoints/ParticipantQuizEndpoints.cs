using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class ParticipantQuizEndpoints
{
    private const string ApiRoute = "/api/participant";

    public static IEndpointRouteBuilder MapParticipantQuizEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder routeGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Participant)));

        MapQuizGetActiveEndpoint(routeGroup);

        return app;
    }

    private static void MapQuizGetActiveEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/available-quizzes", async (int topicIdFilter, IParticipantQuizService quizService) =>
            Results.Ok(await quizService.GetActiveQuizzesAsync(topicIdFilter))
        );
    }
}
