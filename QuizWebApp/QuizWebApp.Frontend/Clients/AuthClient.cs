using System.Net.Http.Json;
using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class AuthClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/auth";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<LoggedInUserInfo>> AddUserTokenAsync(UserLoginDTO userLogin)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{ApiRoute}/login", userLogin);
        QuizApiResponse<LoggedInUserInfo>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<LoggedInUserInfo>>();
        return responseData
            ?? QuizApiResponse<LoggedInUserInfo>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse> RegisterUserAsync(UserSaveDTO saveData)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{ApiRoute}/register", saveData);
        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }
}
