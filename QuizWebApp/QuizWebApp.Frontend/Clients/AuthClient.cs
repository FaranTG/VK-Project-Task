using System.Net.Http.Json;
using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Frontend.Clients;

public class AuthClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/auth/login";

    public async Task<AuthResponseDTO> AddUserTokenAsync(LoginDTO userLogin)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(ApiRoute, userLogin);
        AuthResponseDTO? result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        return result
            ?? new AuthResponseDTO(null, "No response from server.");
    }
}
