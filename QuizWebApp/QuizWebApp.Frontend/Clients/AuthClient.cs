using System.Net.Http.Json;
using QuizWebApp.Shared;
using QuizWebApp.Shared.DTOs;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class AuthClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/auth";
    private const string NoResponseMessage = "No response from server.";

    public async Task<AuthResponseDTO> AddUserTokenAsync(LoginDTO userLogin)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{ApiRoute}/login", userLogin);
        AuthResponseDTO? result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        return result
            ?? new AuthResponseDTO(null, NoResponseMessage);
    }

    public async Task<QuizApiResponse<UserInfoDTO>> RegisterUserAsync(UserSaveDTO data)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{ApiRoute}/register", data);
        QuizApiResponse<UserInfoDTO>? result = await response.Content.ReadFromJsonAsync<QuizApiResponse<UserInfoDTO>>();
        return result
            ?? QuizApiResponse<UserInfoDTO>.Fail(NoResponseMessage);
    }
}
