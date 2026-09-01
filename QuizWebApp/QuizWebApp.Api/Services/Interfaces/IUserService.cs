using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IUserService
{
    public const string NotFoundMessage = "User not found.";

    Task<QuizApiResponse<PagedInfoArray<UserInfoDTO>>> GetUsersAsync(UserApprovedFilter approvedFilter, PaginationDTO paginationData);

    Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int userId);
}
