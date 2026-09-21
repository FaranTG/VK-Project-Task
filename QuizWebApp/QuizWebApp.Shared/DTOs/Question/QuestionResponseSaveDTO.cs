using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Question;

public record QuestionResponseSaveDTO
(
    [Range(1, int.MaxValue)]
    int AttemptId,

    [Range(1, int.MaxValue)]
    int QuestionId,

    [Range(1, int.MaxValue)]
    int SelectedOptionId
);