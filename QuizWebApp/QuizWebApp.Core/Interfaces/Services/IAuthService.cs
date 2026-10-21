using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IAuthService
{
    Task<QuizApiResponse<LoggedInUserInfo>> LoginAsync(UserLoginDTO userData);

    Task<QuizApiResponse> RegisterAsync(UserSaveDTO userData);
}
