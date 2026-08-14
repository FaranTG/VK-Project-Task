using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class LoginModel
{
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}
