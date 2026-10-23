
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Core.Validation;

public static class UserSaveDTOValidator
{
    public static string? Validate(this UserSaveDTO userData, string role)
    {
        if (string.IsNullOrWhiteSpace(userData.Name))
        {
            return "User name is required.";
        }

        if (userData.Name.Length > 20)
        {
            return "User name cannot exceed 20 characters.";
        }

        if (string.IsNullOrWhiteSpace(userData.Phone))
        {
            return "Phone number is required.";
        }

        if (userData.Phone.Length > 15)
        {
            return "Phone number cannot exceed 15 characters.";
        }

        if (string.IsNullOrWhiteSpace(userData.Email))
        {
            return "Email address is required.";
        }

        if (userData.Email.Length > 100)
        {
            return "Email address cannot exceed 100 characters.";
        }

        if (string.IsNullOrWhiteSpace(userData.Password))
        {
            return "Password is required.";
        }

        if (userData.Password.Length > 100)
        {
            return "Password cannot exceed 100 characters.";
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            return "User role is required.";
        }

        if (role != nameof(UserRole.Organizer) && role != nameof(UserRole.Participant))
        {
            return $"Role '{role}' is not allowed. Supported roles are '{nameof(UserRole.Organizer)}' and '{nameof(UserRole.Participant)}'.";
        }

        return null;
    }
}
