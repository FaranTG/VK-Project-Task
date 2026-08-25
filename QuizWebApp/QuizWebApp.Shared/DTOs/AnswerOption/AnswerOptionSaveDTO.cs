using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.AnswerOption;

public record AnswerOptionSaveDTO
(
    [Required][StringLength(50)]
    string Text,

    bool IsCorrect
);