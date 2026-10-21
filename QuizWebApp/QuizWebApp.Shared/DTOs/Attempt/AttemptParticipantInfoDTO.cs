namespace QuizWebApp.Shared.DTOs.Attempt;

public record AttemptParticipantInfoDTO
(
    int Id,

    int ParticipantId,

    string ParticipantName,

    string ParticipantEmail,

    string Status,

    DateTime StartTime,

    DateTime EndTime,

    int Score
);