using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class LoginModel
{
    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    public string? Username { get; set; }

    [Required][StringLength(100)]
    public string? Password { get; set; }
}
