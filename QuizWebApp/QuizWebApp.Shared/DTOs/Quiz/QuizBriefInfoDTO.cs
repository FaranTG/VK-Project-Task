namespace QuizWebApp.Shared.DTOs.Quiz;

public record class QuizBriefInfoDTO
(
    Guid Id,

    string Name,
    
    int TopicId,

    string Topic,

    int QuestionsNumber,

    int TimeInMinutes,

    bool IsActive
);