using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class UsersClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/users";

    public Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, bool? isApprovedFilter)
        => httpClient
            .GetAsync(CreateRequestUrl(paginationData, isApprovedFilter))
            .ExtractQuizApiResponse<PagedInfo<UserInfoDTO>>();

    public Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int id)
        => httpClient
            .PatchAsync($"{ApiRoute}/{id}/toggle-status", null)
            .ExtractQuizApiResponse();

    private static string CreateRequestUrl(PaginationDTO paginationData, bool? isApprovedFilter)
        => isApprovedFilter is null 
            ? $"{ApiRoute}?{paginationData.CreatePaginationParameters()}"
            : $"{ApiRoute}?isApprovedFilter={isApprovedFilter}&{paginationData.CreatePaginationParameters()}";
}
