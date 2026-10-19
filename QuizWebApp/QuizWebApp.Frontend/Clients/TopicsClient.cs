using System.Net.Http.Json;
using QuizWebApp.Frontend.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Frontend.Clients;

public class TopicsClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/topics";

    public Task<QuizApiResponse<PagedInfo<TopicInfoDTO>>> GetTopicsAsync(PaginationDTO? paginationData = null)
        => httpClient
            .GetAsync(paginationData is null ? ApiRoute : $"{ApiRoute}?{paginationData.CreatePaginationParameters()}")
            .ExtractQuizApiResponse<PagedInfo<TopicInfoDTO>>();
    
    public Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id)
        => httpClient
            .GetAsync($"{ApiRoute}/{id}")
            .ExtractQuizApiResponse<TopicInfoDTO>();
    
    public Task<QuizApiResponse<TopicInfoDTO>> AddTopicAsync(TopicSaveDTO newTopic)
        => httpClient
            .PostAsJsonAsync(ApiRoute, newTopic)
            .ExtractQuizApiResponse<TopicInfoDTO>();

    public Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopic)
        => httpClient
            .PutAsJsonAsync($"{ApiRoute}/{id}", newTopic)
            .ExtractQuizApiResponse();
}
