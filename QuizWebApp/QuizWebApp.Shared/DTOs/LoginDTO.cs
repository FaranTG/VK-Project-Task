using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs;

public record LoginDTO
(
    [Required][StringLength(50)]
    string Username,

    [Required][StringLength(100)]
    string Password
);