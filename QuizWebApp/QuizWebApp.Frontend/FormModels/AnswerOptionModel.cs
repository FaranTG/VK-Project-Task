using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class AnswerOptionModel
{
    [Required][StringLength(50)]
    public required string Text { get; set; }

    public bool IsCorrect { get; set; }

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return "Option text is required.";
        }

        if (Text.Length > 50)
        {
            return "The text of the option must not exceed 50 characters.";
        }

        return null;
    }
}
