using System.ComponentModel.DataAnnotations;
using QuizWebApp.Shared.DTOs.Question;

namespace QuizWebApp.Shared.DTOs.Quiz;

public record QuizSaveDTO
(
    [Required][StringLength(25)]
    string Name,

    [Range(1, int.MaxValue)]
    int TopicId,

    [Range(1, 120)]
    int TimeInMinutes,

    bool IsActive,

    [Required][MinLength(1)][MaxLength(20)]
    List<QuestionSaveDTO> Questions
);