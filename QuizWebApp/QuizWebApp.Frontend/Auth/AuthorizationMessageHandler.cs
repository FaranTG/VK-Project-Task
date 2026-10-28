using System.Net;
using System.Net.Http.Headers;

namespace QuizWebApp.Frontend.Auth;

public class AuthorizationMessageHandler(QuizAuthStateProvider authStateProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authStateProvider.IsLoggedIn)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authStateProvider.User!.Token);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await authStateProvider.SetLogoutAsync();
        }

        return response;
    }
}
