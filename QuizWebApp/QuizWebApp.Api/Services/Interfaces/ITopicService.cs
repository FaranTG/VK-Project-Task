using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Api.Services.Interfaces;

public interface ITopicService
{
    Task<QuizApiResponse<TopicInfoDTO[]>> GetTopicsAsync();

    Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id);

    Task<QuizApiResponse<TopicInfoDTO>> CreateTopicAsync(TopicSaveDTO newTopicData);

    Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopicData);
}
