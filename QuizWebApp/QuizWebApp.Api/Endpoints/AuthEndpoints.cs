using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Endpoints;

public static class AuthEndpoints
{
    private const string ApiRoute = "/api/auth";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder authRouteGroup = app.MapGroup(ApiRoute);

        authRouteGroup.MapPost("/login", async (UserLoginDTO data, IAuthService authService) =>
        {
            QuizApiResponse<LoggedInUserInfo> response = await authService.LoginAsync(data);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });

        authRouteGroup.MapPost("/register", async (UserSaveDTO data, IAuthService authService) =>
        {
            QuizApiResponse response = await authService.RegisterAsync(data);

            return response.IsSuccess 
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });

        return app;
    }
}
