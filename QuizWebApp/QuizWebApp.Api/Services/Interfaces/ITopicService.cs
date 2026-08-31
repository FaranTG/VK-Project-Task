using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Api.Services.Interfaces;

public interface ITopicService
{
    public const string NotFoundMessage = "Topic not found.";

    Task<QuizApiResponse<TopicInfoDTO[]>> GetTopicsAsync();

    Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id);

    Task<QuizApiResponse<TopicInfoDTO>> CreateTopicAsync(TopicSaveDTO newTopicData);

    Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopicData);
}
