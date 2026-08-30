using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Endpoints;

public static class AuthEndpoints
{
    private const string ApiRoute = "/api/auth";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder authRouteGroup = app.MapGroup(ApiRoute);

        authRouteGroup.MapPost("/login", async (UserLoginDTO data, IAuthService authService) => 
            Results.Ok(await authService.LoginAsync(data))
        );

        authRouteGroup.MapPost("/register", async (UserSaveDTO data, IAuthService authService) =>
            Results.Ok(await authService.RegisterAsync(data))
        );

        return app;
    }
}
