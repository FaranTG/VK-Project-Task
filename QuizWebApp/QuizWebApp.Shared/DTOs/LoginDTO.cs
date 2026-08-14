using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs;

public record class LoginDTO
(
    [Required]
    string Username,

    [Required]
    string Password
);