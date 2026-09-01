using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class UserEndpoints
{
    private const string ApiRoute = "/api/users";

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder organizerRouteGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapUserGetEndpoint(organizerRouteGroup);
        MapUserPatchEndpoint(organizerRouteGroup);

        return app;
    }

    private static void MapUserGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (UserApprovedFilter approvedFilter, [AsParameters] PaginationDTO paginationData, IUserService userService) => 
            Results.Ok(await userService.GetUsersAsync(approvedFilter, paginationData))
        );
    }

    private static void MapUserPatchEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/{id:int}/toggle-status", async (int id, IUserService userService) =>
        {
            QuizApiResponse response = await userService.ToggleUserApprovedStatusAsync(id);
            
            if (response.IsFailure)
            {
                return response.ErrorMessage == IUserService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}
