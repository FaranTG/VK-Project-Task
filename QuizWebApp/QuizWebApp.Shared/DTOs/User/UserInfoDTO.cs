namespace QuizWebApp.Shared.DTOs.User;

public record UserInfoDTO
(
    int Id,

    string Name,

    string Phone,

    string Email,

    string Role,
    
    bool IsApproved
);