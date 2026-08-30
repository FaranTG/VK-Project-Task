using System.Net.Http.Json;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Frontend.Clients;

public class TopicsClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/topics";
    private const string NoResponseMessage = "No response from server.";

    public async Task<QuizApiResponse<TopicInfoDTO[]>> GetTopicsAsync()
    {
        HttpResponseMessage response = await httpClient.GetAsync(ApiRoute);

        QuizApiResponse<TopicInfoDTO[]>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<TopicInfoDTO[]>>();
        
        return responseData
            ?? QuizApiResponse<TopicInfoDTO[]>.Fail(NoResponseMessage);
    }
    
    public async Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"{ApiRoute}/{id}");

        QuizApiResponse<TopicInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<TopicInfoDTO>>();
        return responseData
            ?? QuizApiResponse<TopicInfoDTO>.Fail(NoResponseMessage);
    }
    
    public async Task<QuizApiResponse<TopicInfoDTO>> AddTopicAsync(TopicSaveDTO newTopic)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(ApiRoute, newTopic);

        QuizApiResponse<TopicInfoDTO>? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse<TopicInfoDTO>>();
        return responseData
            ?? QuizApiResponse<TopicInfoDTO>.Fail(NoResponseMessage);
    }

    public async Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopic)
    {
        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{ApiRoute}/{id}", newTopic);

        QuizApiResponse? responseData = await response.Content.ReadFromJsonAsync<QuizApiResponse>();
        return responseData
            ?? QuizApiResponse.Fail(NoResponseMessage);
    }
}
