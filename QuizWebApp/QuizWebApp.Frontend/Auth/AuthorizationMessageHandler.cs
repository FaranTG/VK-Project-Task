using System.Net.Http.Headers;

namespace QuizWebApp.Frontend.Auth;

public class AuthorizationMessageHandler(QuizAuthStateProvider authStateProvider) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authStateProvider.IsLoggedIn)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authStateProvider.User!.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
