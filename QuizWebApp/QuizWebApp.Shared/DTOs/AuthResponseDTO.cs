namespace QuizWebApp.Shared.DTOs;

public record AuthResponseDTO
(
    LoggedInUser? User,
    string? ErrorMessage
);