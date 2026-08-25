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

        builder.Services.AddTransient<AuthorizationMessageHandler>();
        
        AddAuthClient(builder, baseUri);
        AddTopicsClient(builder, baseUri);
        AddQuizzesClient(builder, baseUri);
    }

    private static void AddAuthClient(WebAssemblyHostBuilder builder, Uri baseUri)
    {
        builder.Services.AddScoped
        (
            sp => new AuthClient(new HttpClient { BaseAddress = baseUri })
        );
    }

    private static void AddTopicsClient(WebAssemblyHostBuilder builder, Uri baseUri)
    {
        builder.Services.AddScoped
        (
            sp => new TopicsClient(CreateAuthorizedClient(sp, baseUri))
        );
    }

    private static void AddQuizzesClient(WebAssemblyHostBuilder builder, Uri baseUri)
    {
        builder.Services.AddScoped
        (
            sp => new QuizzesClient(CreateAuthorizedClient(sp, baseUri))
        );
    }

    private static HttpClient CreateAuthorizedClient(IServiceProvider serviceProvider, Uri baseUri)
    {
        AuthorizationMessageHandler handler = serviceProvider.GetRequiredService<AuthorizationMessageHandler>();
        handler.InnerHandler = new HttpClientHandler();

        return new (handler) { BaseAddress = baseUri };
    }
}
