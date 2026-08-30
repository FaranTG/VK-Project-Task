using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.User;

public record UserSaveDTO
(
    [Required][StringLength(20)]
    string Name,

    [Required][Phone][StringLength(15)]
    string Phone,

    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    string Email,

    [Required][StringLength(100)]
    string Password
);