using QuizWebApp.Shared.ApiResponses;

namespace QuizWebApp.Shared.DTOs.Attempt;

public record QuizAttemptsInfoDTO
(
    string QuizName,

    string TopicName,

    PagedInfoArray<AttemptParticipantInfoDTO> Attempts
);