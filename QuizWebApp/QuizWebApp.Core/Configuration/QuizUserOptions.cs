namespace QuizWebApp.Core.Configuration;

public class QuizUserOptions
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required string Password { get; set; }
}