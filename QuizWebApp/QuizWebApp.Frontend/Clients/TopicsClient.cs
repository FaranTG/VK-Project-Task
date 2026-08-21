using System.Net.Http.Json;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Frontend.Clients;

public class TopicsClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/topics";

    public async Task<TopicInfoDTO[]> GetTopicsAsync()
        => await httpClient.GetFromJsonAsync<TopicInfoDTO[]>(ApiRoute)
        ?? throw new InvalidOperationException("Could not find topics list.");
    
    public async Task<TopicInfoDTO> GetTopicByIdAsync(int id)
        => await httpClient.GetFromJsonAsync<TopicInfoDTO>($"{ApiRoute}/{id}")
        ?? throw new InvalidOperationException("Could not find topic.");
    
    public async Task AddTopicAsync(TopicSaveDTO newTopic)
        => await httpClient.PostAsJsonAsync(ApiRoute, newTopic);

    public async Task UpdateTopicAsync(int id, TopicSaveDTO newTopic)
        => await httpClient.PutAsJsonAsync($"{ApiRoute}/{id}", newTopic);

    public async Task DeleteTopicAsync(int id)
        => await httpClient.DeleteAsync($"{ApiRoute}/{id}");
}
