namespace QuizWebApp.Shared.DTOs.Attempt;

public record AttemptParticipantInfoDTO
(
    int Id,

    int ParticipantId,

    string ParticipantName,

    string ParticipantEmail,

    string Status,

    DateTimeOffset StartTime,

    DateTimeOffset EndTime,

    int Score
);