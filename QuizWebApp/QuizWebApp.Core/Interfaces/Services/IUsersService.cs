using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Core.Interfaces.Services;

public interface IUsersService
{
    public const string NotFoundMessage = "User not found.";

    Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, bool? isApprovedFilter);

    Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int userId);
}
