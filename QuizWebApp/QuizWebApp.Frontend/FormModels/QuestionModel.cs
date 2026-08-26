using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class QuestionModel
{
    [Required][StringLength(500)]
    public required string Text { get; set; }

    [Required][MinLength(2)][MaxLength(8)]
    public required List<AnswerOptionModel> Options { get; set; }

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return "Question text is required.";
        }

        if (Text.Length > 500)
        {
            return "The text of the question must not exceed 500 characters.";
        }

        if (Options.Count < 2 || Options.Count > 8)
        {
            return "The number of options in a question must be no less than 2 and no more than 8.";
        }

        int correctOptionsCount = 0;
        foreach (AnswerOptionModel option in Options)
        {
            string? validationResult = option.Validate();
            if (validationResult is null)
            {
                if (option.IsCorrect)
                {
                    ++correctOptionsCount;
                }

                continue;
            }

            return validationResult;
        }

        if (correctOptionsCount != 1)
        {
            return "There should be only one correct option in the question";
        }

        return null;
    }
}
