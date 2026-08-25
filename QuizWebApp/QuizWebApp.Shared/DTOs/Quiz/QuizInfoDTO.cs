using QuizWebApp.Shared.DTOs.Question;

namespace QuizWebApp.Shared.DTOs.Quiz;

public record QuizInfoDTO
(
    Guid Id,

    string Name,

    int TopicId,

    int QuestionsNumber,

    int TimeInMinutes,

    bool IsActive,

    List<QuestionInfoDTO> Questions
);