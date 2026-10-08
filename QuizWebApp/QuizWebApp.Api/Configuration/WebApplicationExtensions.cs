using QuizWebApp.Api.Endpoints;

namespace QuizWebApp.Api.Configuration;

public static class WebApplicationExtensions
{
    public static IEndpointRouteBuilder MapAllQuizEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapAuthEndpoints()
            .MapTopicEndpoints()
            .MapQuizEndpoints()
            .MapAttemptEndpoints()
            .MapUserEndpoints()
            .MapOrganizerEndpoints();
        
        return app;
    }
}
