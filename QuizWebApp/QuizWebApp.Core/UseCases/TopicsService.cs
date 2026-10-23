using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Core.UseCases;

public class TopicsService : ITopicsService
{
    private const string DuplicateMessage = "A topic with this name already exists.";
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 100;

    private readonly ITopicsRepository _topicsRepository;

    public TopicsService(ITopicsRepository topicsRepository)
    {
        _topicsRepository = topicsRepository;
    }

    public async Task<QuizApiResponse<PagedInfo<TopicInfoDTO>>> GetTopicsAsync(PaginationDTO? paginationData)
    {
        try
        {
            paginationData ??= new PaginationDTO(DefaultPageNumber, DefaultPageSize);

            PagedInfo<Topic> pagedTopics = await _topicsRepository
                .GetTopicsAsync(paginationData);

            TopicInfoDTO[] topicDTOs = pagedTopics.DTOs
                .Select(CreateTopicInfoDTO)
                .ToArray();
            
            PagedInfo<TopicInfoDTO> result = new (topicDTOs, pagedTopics.TotalCount);
            
            return QuizApiResponse<PagedInfo<TopicInfoDTO>>.Success(result);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<TopicInfoDTO>>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<TopicInfoDTO>> GetTopicByIdAsync(int id)
    {
        try
        {
            Topic? topic = await _topicsRepository
                .GetByIdAsync(id);

            return topic is null
                ? QuizApiResponse<TopicInfoDTO>.Fail(ITopicsService.NotFoundMessage)
                : QuizApiResponse<TopicInfoDTO>.Success(CreateTopicInfoDTO(topic));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<TopicInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse<TopicInfoDTO>> CreateTopicAsync(TopicSaveDTO newTopicData)
    {
        try
        {
            bool isTopicDuplicate = await _topicsRepository
                .ExistsByNameAsync(newTopicData.Name);

            if (isTopicDuplicate)
            {
                return QuizApiResponse<TopicInfoDTO>.Fail(DuplicateMessage);
            }

            Topic topic = new () { Name = newTopicData.Name };

            await _topicsRepository
                .AddAsync(topic);

            return QuizApiResponse<TopicInfoDTO>.Success(CreateTopicInfoDTO(topic));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<TopicInfoDTO>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> UpdateTopicAsync(int id, TopicSaveDTO newTopicData)
    {
        try
        {
            Topic? topic = await _topicsRepository
                .GetByIdAsync(id);

            if (topic is null)
            {
                return QuizApiResponse.Fail(ITopicsService.NotFoundMessage);
            }

            bool isNameDuplicate = await _topicsRepository
                .ExistsByNameWithDifferentIdAsync(newTopicData.Name, id);

            if (isNameDuplicate)
            {
                return QuizApiResponse.Fail(DuplicateMessage);
            }

            topic.Name = newTopicData.Name;
            
            await _topicsRepository
                .UpdateAsync(topic);

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private TopicInfoDTO CreateTopicInfoDTO(Topic topic)
        => new (topic.Id, topic.Name);
}
