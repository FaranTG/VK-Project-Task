namespace QuizWebApp.Shared.DTOs.Quiz;

public record class QuizSummaryInfoDTO
(
    Guid Id,

    string Name,
    
    int TopicId,

    string Topic,

    int QuestionsNumber,

    int TimeInMinutes,

    bool IsActive,
    
    List<string> Questions
);