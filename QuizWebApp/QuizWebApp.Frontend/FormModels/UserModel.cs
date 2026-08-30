using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class UserModel
{
    [Required][StringLength(20)]
    public required string Name { get; set; }

    [Required][Phone][StringLength(15)]
    public required string Phone { get; set; }

    [Required][EmailAddress][DataType(DataType.EmailAddress)][StringLength(100)]
    public required string Email { get; set; }

    [Required][StringLength(100)]
    public required string Password { get; set; }
}
