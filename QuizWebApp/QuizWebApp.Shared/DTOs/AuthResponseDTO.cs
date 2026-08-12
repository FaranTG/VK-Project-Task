namespace QuizWebApp.Shared.DTOs;

public record class AuthResponseDTO
(
    string? Token,
    string? ErrorMessage
);