using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class UsersEndpoints
{
    private const string ApiRoute = "/api/users";

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
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
        app.MapGet("/", async (bool? isApprovedFilter, [AsParameters] PaginationDTO paginationData, IUsersService userService) => 
            Results.Ok(await userService.GetUsersAsync(paginationData, isApprovedFilter))
        );
    }

    private static void MapUserPatchEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/{id:int}/toggle-status", async (int id, IUsersService userService) =>
        {
            QuizApiResponse response = await userService.ToggleUserApprovedStatusAsync(id);
            
            if (response.IsFailure)
            {
                return response.ErrorMessage == IUsersService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}
