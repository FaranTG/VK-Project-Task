using QuizWebApp.Shared.DTOs.AnswerOption;

namespace QuizWebApp.Shared.DTOs.Question;

public record QuestionInfoDTO
(
    int Id,

    string Text,

    List<AnswerOptionInfoDTO> Options
);