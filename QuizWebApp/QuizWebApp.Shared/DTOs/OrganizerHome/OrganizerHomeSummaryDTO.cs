namespace QuizWebApp.Shared.DTOs.OrganizerHome;

public record OrganizerHomeSummaryDTO
(
    int TopicsCount,

    int ParticipantsCount,

    int ApprovedParticipantsCount,

    int QuizzesCount,
    
    int ActiveQuizzesCount
);