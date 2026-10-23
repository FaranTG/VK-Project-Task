using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Frontend.Clients;

public class UsersClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/users";

    public Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, UserFilterDTO filterData)
        => httpClient
            .GetAsync(CreateGetUsersRequestUrl(paginationData, filterData))
            .ExtractQuizApiResponse<PagedInfo<UserInfoDTO>>();

    public Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int id)
        => httpClient
            .PatchAsync($"{ApiRoute}/{id}/toggle-status", null)
            .ExtractQuizApiResponse();

    private static string CreateGetUsersRequestUrl(PaginationDTO paginationData, UserFilterDTO filterData)
    {
        string url = $"{ApiRoute}?{paginationData.CreatePaginationParameters()}";

        if (!string.IsNullOrWhiteSpace(filterData.SearchFilter))
        {
            url += $"&searchFilter={Uri.EscapeDataString(filterData.SearchFilter.Trim())}";
        }

        if (filterData.IsApprovedFilter is not null)
        {
            url += $"&isApprovedFilter={filterData.IsApprovedFilter.Value.ToString().ToLower()}";
        }

        return url;
    }
}
