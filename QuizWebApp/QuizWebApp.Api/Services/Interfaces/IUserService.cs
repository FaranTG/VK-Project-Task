using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Services.Interfaces;

public interface IUserService
{
    public const string NotFoundMessage = "User not found.";

    Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, bool? isApprovedFilter);

    Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int userId);
}
