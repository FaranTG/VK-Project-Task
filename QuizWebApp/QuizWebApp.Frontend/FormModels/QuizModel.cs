using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class QuizModel
{
    [Required][StringLength(100)]
    public required string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "The Topic field is required.")]
    public int TopicId { get; set; }

    [Range(1, 120)]
    public int TimeInMinutes { get; set; }

    public bool IsActive { get; set; }

    [Required][MinLength(1)][MaxLength(50)]
    public required List<QuestionModel> Questions { get; set; }

    public string? Validate()
    {
        foreach (QuestionModel question in Questions)
        {
            string? validationResult = question.Validate();
            if (validationResult is null)
            {
                continue; 
            }

            return validationResult;
        }

        return null;
    }
}
