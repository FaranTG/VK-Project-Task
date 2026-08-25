using System.ComponentModel.DataAnnotations;
using QuizWebApp.Shared.DTOs.AnswerOption;

namespace QuizWebApp.Shared.DTOs.Question;

public record QuestionSaveDTO
(
    [Required][StringLength(500)]
    string Text,

    [Required][MinLength(2)][MaxLength(8)]
    List<AnswerOptionSaveDTO> Options
);