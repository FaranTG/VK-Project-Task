using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Core.Interfaces.Services;

public interface ITopicsService
{
    public const string NotFoundMessage = "Topic not found.";

    Task<QuizApiResponse<PagedInfo<TopicInfoDTO>>> GetTopicsAsync(PaginationDTO? paginationData, TopicFilterDTO filterData);

    Task<QuizApiResponse<TopicInfoDTO>> CreateTopicAsync(TopicSaveDTO newTopicData);

    Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopicData);
}
