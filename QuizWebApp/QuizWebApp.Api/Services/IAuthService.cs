using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Services;

public interface IAuthService
{
    Task<QuizApiResponse<LoggedInUserInfo>> LoginAsync(UserLoginDTO data);

    Task<QuizApiResponse> RegisterAsync(UserSaveDTO userData);
}
