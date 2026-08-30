using QuizWebApp.Api.Services;
using QuizWebApp.Shared.DTOs;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Endpoints;

public static class AuthEndpoints
{
    private const string ApiRoute = "/api/auth";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost($"{ApiRoute}/login", async (LoginDTO data, IAuthService authService) => 
            Results.Ok(await authService.LoginAsync(data))
        );

        app.MapPost($"{ApiRoute}/register", async (UserSaveDTO data, IAuthService authService) =>
            Results.Ok(await authService.RegisterAsync(data))
        );

        return app;
    }
}
