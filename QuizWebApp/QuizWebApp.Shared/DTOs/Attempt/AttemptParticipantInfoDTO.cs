namespace QuizWebApp.Shared.DTOs.Attempt;

public record AttemptParticipantInfoDTO
(
    int Id,

    int ParticipantId,

    string ParticipantName,

    string Status,

    DateTime StartTime,

    DateTime EndTime,

    int Score
);