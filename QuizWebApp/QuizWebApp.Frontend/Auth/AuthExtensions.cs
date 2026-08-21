using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace QuizWebApp.Frontend.Auth;

public static class AuthExtensions
{
    public static void AddQuizAuth(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddSingleton<QuizAuthStateProvider>();

        builder.Services.AddSingleton<AuthenticationStateProvider>(
            sp => sp.GetRequiredService<QuizAuthStateProvider>()
        );
        
        builder.Services.AddAuthorizationCore();
    }
}
