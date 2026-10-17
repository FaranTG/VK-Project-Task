using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Frontend.Clients;

public class UsersClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/users";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(UserApprovedFilter approvedFilter, PaginationDTO paginationData)
    {
        string requestUrl = $"{ApiRoute}?approvedFilter={approvedFilter}&pageNumber={paginationData.PageNumber}&pageSize={paginationData.PageSize}";
        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

        QuizApiResponse<PagedInfo<UserInfoDTO>>? responseData = await response.Content
            .ReadFromJsonAsync<QuizApiResponse<PagedInfo<UserInfoDTO>>>();
        
        return responseData
            ?? QuizApiResponse<PagedInfo<UserInfoDTO>>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int id)
    {
        HttpResponseMessage response = await httpClient.PatchAsync($"{ApiRoute}/{id}/toggle-status", null);

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();

        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }
}
