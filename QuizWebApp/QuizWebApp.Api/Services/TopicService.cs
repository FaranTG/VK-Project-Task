using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Api.Services;

public class TopicService : ITopicService
{
    private const string DuplicateMessage = "A topic with this name already exists.";

    private readonly QuizContext _dbContext;

    public TopicService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<PagedInfo<TopicInfoDTO>>> GetTopicsAsync(PaginationDTO? paginationData)
    {
        try
        {
            paginationData ??= new PaginationDTO(1, 100);

            IQueryable<Topic> query = _dbContext.Topics.AsNoTracking();
            int totalCount = await query.CountAsync();

            TopicInfoDTO[] topics = await query
                .OrderBy(topic => topic.Id)
                .Skip((paginationData.PageNumber - 1) * paginationData.PageSize)
                .Take(paginationData.PageSize)
                .Select(
                    topic => new TopicInfoDTO
                    (
                        topic.Id,
                        topic.Name
                    )
                )
                .ToArrayAsync();
            
            PagedInfo<TopicInfoDTO> pagedTopics = new (topics, totalCount);
            
            return QuizApiResponse<PagedInfo<TopicInfoDTO>>.Success(pagedTopics);
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
            TopicInfoDTO? topic = await _dbContext.Topics
                .AsNoTracking()
                .Select(topic => new TopicInfoDTO(topic.Id, topic.Name))
                .FirstOrDefaultAsync(topic => topic.Id == id);

            return topic is null
                ? QuizApiResponse<TopicInfoDTO>.Fail(ITopicService.NotFoundMessage)
                : QuizApiResponse<TopicInfoDTO>.Success(topic);
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
            if (await _dbContext.Topics.AnyAsync(topic => topic.Name == newTopicData.Name))
            {
                return QuizApiResponse<TopicInfoDTO>.Fail(DuplicateMessage);
            }

            Topic topic = new ()  { Name = newTopicData.Name };

            _dbContext.Topics.Add(topic);
            await _dbContext.SaveChangesAsync();

            TopicInfoDTO createdTopicData = new (topic.Id, topic.Name);
            return QuizApiResponse<TopicInfoDTO>.Success(createdTopicData);
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
            Topic? topic = await _dbContext.Topics.FindAsync(id);

            if (topic is null)
            {
                return QuizApiResponse.Fail(ITopicService.NotFoundMessage);
            }

            if (await _dbContext.Topics.AnyAsync(topic => topic.Name == newTopicData.Name && topic.Id != id))
            {
                return QuizApiResponse.Fail(DuplicateMessage);
            }

            topic.Name = newTopicData.Name;
            
            await _dbContext.SaveChangesAsync();

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }
}
