using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class UsersClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/users";

    public Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, string? searchFilter, bool? isApprovedFilter)
        => httpClient
            .GetAsync(CreateGetUsersRequestUrl(paginationData, searchFilter, isApprovedFilter))
            .ExtractQuizApiResponse<PagedInfo<UserInfoDTO>>();

    public Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int id)
        => httpClient
            .PatchAsync($"{ApiRoute}/{id}/toggle-status", null)
            .ExtractQuizApiResponse();

    private static string CreateGetUsersRequestUrl(PaginationDTO paginationData, string? searchFilter, bool? isApprovedFilter)
    {
        string url = $"{ApiRoute}?{paginationData.CreatePaginationParameters()}";

        if (isApprovedFilter is not null)
        {
            url += $"&isApprovedFilter={isApprovedFilter.Value.ToString().ToLower()}";
        }

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            url += $"&searchFilter={Uri.EscapeDataString(searchFilter.Trim())}";
        }

        return url;
    }
}
