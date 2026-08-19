using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs;

public record LoginDTO
(
    [Required]
    string Username,

    [Required]
    string Password
);