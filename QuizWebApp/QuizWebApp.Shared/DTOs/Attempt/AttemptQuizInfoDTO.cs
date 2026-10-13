namespace QuizWebApp.Shared.DTOs.Attempt;

public record AttemptQuizInfoDTO
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