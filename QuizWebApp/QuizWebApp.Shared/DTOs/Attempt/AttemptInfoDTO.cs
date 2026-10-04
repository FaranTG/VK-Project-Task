namespace QuizWebApp.Shared.DTOs.Attempt;

public record AttemptInfoDTO
(
    int Id,

    Guid QuizId,

    string QuizName,

    string TopicName,

    string Status,

    DateTime StartTime,

    DateTime EndTime,

    int Score
);