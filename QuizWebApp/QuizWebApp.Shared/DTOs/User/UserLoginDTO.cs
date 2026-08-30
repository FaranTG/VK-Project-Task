using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.User;

public record UserLoginDTO
(
    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    string Username,

    [Required][StringLength(100)]
    string Password
);