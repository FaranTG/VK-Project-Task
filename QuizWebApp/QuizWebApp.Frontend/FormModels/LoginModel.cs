using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class LoginModel
{
    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    public required string Username { get; set; }

    [Required][StringLength(100)]
    public required string Password { get; set; }
}
