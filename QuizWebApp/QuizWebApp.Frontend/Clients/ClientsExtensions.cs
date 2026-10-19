using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuizWebApp.Frontend.Auth;

namespace QuizWebApp.Frontend.Clients;

public static class ClientsExtensions
{
    public static void AddClients(this WebAssemblyHostBuilder builder)
    {
        string apiConnectionStringName = "QuizApiUrl";
        string quizApiUrl = builder.Configuration.GetConnectionString(apiConnectionStringName)
            ?? throw new InvalidOperationException($"Configuration value with name '{apiConnectionStringName}' does not exist");

        Uri baseUri = new (quizApiUrl);

        builder.Services
            .AddTransient<AuthorizationMessageHandler>()
            .AddAuthClient(baseUri)
            .AddAuthorizedClient(baseUri, client => new TopicsClient(client))
            .AddAuthorizedClient(baseUri, client => new QuizzesClient(client))
            .AddAuthorizedClient(baseUri, client => new AttemptsClient(client))
            .AddAuthorizedClient(baseUri, client => new UsersClient(client))
            .AddAuthorizedClient(baseUri, client => new OrganizerClient(client));
    }

    private static IServiceCollection AddAuthClient(this IServiceCollection services, Uri baseUri)
    {
        return services.AddScoped
        (
            sp => new AuthClient(new HttpClient { BaseAddress = baseUri })
        );
    }

    private static IServiceCollection AddAuthorizedClient<TClient>(this IServiceCollection services,
        Uri baseUri, Func<HttpClient, TClient> clientFactory)
        where TClient : class
    {
        return services.AddScoped
        (
            sp => clientFactory(CreateAuthorizedClient(sp, baseUri))
        );
    }

    private static HttpClient CreateAuthorizedClient(IServiceProvider serviceProvider, Uri baseUri)
    {
        AuthorizationMessageHandler handler = serviceProvider.GetRequiredService<AuthorizationMessageHandler>();
        handler.InnerHandler = new HttpClientHandler(); 

        return new (handler) { BaseAddress = baseUri };
    }
}
