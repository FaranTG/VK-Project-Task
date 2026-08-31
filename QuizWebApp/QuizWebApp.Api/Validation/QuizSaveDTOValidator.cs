using QuizWebApp.Shared.DTOs.AnswerOption;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Api.Validation;

public static class QuizSaveDTOValidator
{
    public static string? Validate(this QuizSaveDTO quizData)
    {
        foreach (QuestionSaveDTO question in quizData.Questions)
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

    private static string? Validate(this QuestionSaveDTO questionData)
    {
        if (string.IsNullOrWhiteSpace(questionData.Text))
        {
            return "Question text is required.";
        }

        if (questionData.Text.Length > 500)
        {
            return "The text of the question must not exceed 500 characters.";
        }

        if (questionData.Options is null || questionData.Options.Count < 2 || questionData.Options.Count > 8)
        {
            return "The number of options in a question must be no less than 2 and no more than 8.";
        }

        int correctOptionsCount = 0;
        foreach (AnswerOptionSaveDTO option in questionData.Options)
        {
            string? validationResult = option.Validate();
            if (validationResult is not null)
            {
                return validationResult;
            }

            if (option.IsCorrect)
            {
                ++correctOptionsCount;
            }
        }

        if (correctOptionsCount != 1)
        {
            return "There should be only one correct option in the question";
        }

        return null;
    }

    private static string? Validate(this AnswerOptionSaveDTO optionData)
    {
        if (string.IsNullOrWhiteSpace(optionData.Text))
        {
            return "Option text is required.";
        }

        if (optionData.Text.Length > 50)
        {
            return "The text of the option must not exceed 50 characters.";
        }

        return null;
    }
}
