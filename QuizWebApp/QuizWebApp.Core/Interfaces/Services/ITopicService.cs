using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Core.Interfaces.Services;

public interface ITopicService
{
    public const string NotFoundMessage = "Topic not found.";

    Task<QuizApiResponse<PagedInfo<TopicInfoDTO>>> GetTopicsAsync(PaginationDTO? paginationData);

    Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id);

    Task<QuizApiResponse<TopicInfoDTO>> CreateTopicAsync(TopicSaveDTO newTopicData);

    Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopicData);
}
