using QuizWebApp.Api.Services;
using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Api.Endpoints;

public static class AuthEndpoints
{
    private const string ApiRoute = "/api/auth/login";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoute, async (LoginDTO data, IAuthService authService) => 
            Results.Ok(await authService.LoginAsync(data))
        );

        return app;
    }
}
