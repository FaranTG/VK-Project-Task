using QuizWebApp.Shared;
using QuizWebApp.Shared.DTOs;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginDTO data);

    Task<QuizApiResponse<UserInfoDTO>> RegisterAsync(UserSaveDTO userData);
}
