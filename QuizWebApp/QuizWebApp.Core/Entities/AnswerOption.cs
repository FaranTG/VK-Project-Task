using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Core.Entities;

public class AnswerOption
{
    public int Id { get; set; }

    [Required][StringLength(50)]
    public required string Text { get; set; }

    public int QuestionId { get; set; }

    public Question? Question { get; set; }

    public bool IsCorrect { get; set; }
}
