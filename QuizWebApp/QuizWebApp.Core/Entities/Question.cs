using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Core.Entities;

public class Question
{
    public int Id { get; set; }

    [Required][StringLength(500)]
    public required string Text { get; set; }

    public Guid QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    public required List<AnswerOption> Options { get; set; }
}
