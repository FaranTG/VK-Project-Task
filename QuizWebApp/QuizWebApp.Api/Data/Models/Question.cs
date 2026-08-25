namespace QuizWebApp.Api.Data.Models;

public class Question
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public Guid QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    public required ICollection<AnswerOption> Options { get; set; }
}
