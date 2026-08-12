using QuizWebApp.Api.Services;
using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginDTO data, IAuthService authService) => 
            Results.Ok(await authService.LoginAsync(data))
        );

        return app;
    }
}
