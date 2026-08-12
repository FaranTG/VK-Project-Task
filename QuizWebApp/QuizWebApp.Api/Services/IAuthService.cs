using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginDTO data);
}
