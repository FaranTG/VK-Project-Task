using QuizWebApp.Api.Endpoints;
using QuizWebApp.Infrastructure.Data;

namespace QuizWebApp.Api.Extensions;

public static class ConfigurationExtensions
{
    public static void InitializeDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        
        DatabaseInitializer initializer = scope.ServiceProvider
            .GetRequiredService<DatabaseInitializer>();

        initializer.Initialize();
    }

    public static IEndpointRouteBuilder MapAllQuizEndpoints(this IEndpointRouteBuilder app)
        => app
            .MapAuthEndpoints()
            .MapTopicEndpoints()
            .MapQuizEndpoints()
            .MapAttemptEndpoints()
            .MapUserEndpoints()
            .MapOrganizerEndpoints();
}
