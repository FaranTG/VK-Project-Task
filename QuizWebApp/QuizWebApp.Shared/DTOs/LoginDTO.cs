using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs;

public record LoginDTO
(
    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    string Username,

    [Required][StringLength(100)]
    string Password
);