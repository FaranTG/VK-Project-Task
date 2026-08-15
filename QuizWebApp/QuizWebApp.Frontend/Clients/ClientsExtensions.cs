using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace QuizWebApp.Frontend.Clients;

public static class ClientsExtensions
{
    public static void AddClients(this WebAssemblyHostBuilder builder)
    {
        string apiConnectionStringName = "QuizApiUrl";
        string quizApiUrl = builder.Configuration.GetConnectionString(apiConnectionStringName)
            ?? throw new InvalidOperationException($"Configuration value with name '{apiConnectionStringName}' does not exist");

        Uri baseUri = new (quizApiUrl);

        builder.Services.AddScoped
        (
            sp => new AuthClient(new HttpClient { BaseAddress = baseUri })
        );
    }
}
