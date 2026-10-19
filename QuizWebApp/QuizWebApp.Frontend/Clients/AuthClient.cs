using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class AuthClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/auth";

    public Task<QuizApiResponse<LoggedInUserInfo>> AddUserTokenAsync(UserLoginDTO userLogin)
        => httpClient
            .PostAsJsonAsync($"{ApiRoute}/login", userLogin)
            .ExtractQuizApiResponse<LoggedInUserInfo>();

    public Task<QuizApiResponse> RegisterUserAsync(UserSaveDTO saveData)
        => httpClient
            .PostAsJsonAsync($"{ApiRoute}/register", saveData)
            .ExtractQuizApiResponse();
}