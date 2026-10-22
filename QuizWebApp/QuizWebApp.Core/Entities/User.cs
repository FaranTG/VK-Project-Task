using System.ComponentModel.DataAnnotations;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Core.Entities;

public class User
{
    public int Id { get; set; }

    [Required][StringLength(20)]
    public required string Name { get; set; }

    [Required][Phone][StringLength(15)]
    public required string Phone { get; set; }

    [Required][EmailAddress][StringLength(100)]
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }
    
    [AllowedValues(
        nameof(UserRole.Organizer),
        nameof(UserRole.Participant)
    )]
    public required string Role { get; set; }

    public bool IsApproved { get; set; }
}
