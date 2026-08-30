using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Api.Services;

public class TopicService : ITopicService
{
    private readonly QuizContext _dbContext;

    public TopicService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<TopicInfoDTO[]>> GetTopicsAsync()
    {
        try
        {
            TopicInfoDTO[] topics = await _dbContext.Topics
                .AsNoTracking()
                .OrderBy(topic => topic.Id)
                .Select(
                    topic => new TopicInfoDTO
                    (
                        topic.Id,
                        topic.Name
                    )
                )
                .ToArrayAsync();
            
            return QuizApiResponse<TopicInfoDTO[]>.Success(topics);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<TopicInfoDTO[]>.Fail(exception.Message);
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
            ? QuizApiResponse<TopicInfoDTO>.Fail("Topic not found.")
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
                return QuizApiResponse<TopicInfoDTO>.Fail("A topic with this name already exists.");
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
                return QuizApiResponse.Fail("Topic not found.");
            }

            if (await _dbContext.Topics.AnyAsync(topic => topic.Name == newTopicData.Name && topic.Id != id))
            {
                return QuizApiResponse.Fail("A topic with this name already exists.");
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
