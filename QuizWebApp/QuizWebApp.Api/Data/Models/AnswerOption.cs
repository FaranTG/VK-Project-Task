namespace QuizWebApp.Api.Data.Models;

public class AnswerOption
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public int QuestionId { get; set; }

    public Question? Question { get; set; }

    public bool IsCorrect { get; set; }
}
