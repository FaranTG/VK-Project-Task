using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class OrganizerEndpoints
{
    private const string ApiRoute = "/api/organizer";

    public static IEndpointRouteBuilder MapOrganizerEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder routeGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapSummaryGetEndpoint(routeGroup);

        return app;
    }

    private static void MapSummaryGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/summary", async (IOrganizerService organizerService) =>
            Results.Ok(await organizerService.GetHomeSummaryAsync())
        );
    }
}
