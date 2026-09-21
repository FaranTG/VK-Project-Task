namespace QuizWebApp.Api.Data.Models;

public class AttemptQuestion
{
    public int AttemptId { get; set; }

    public Attempt? Attempt { get; set; }

    public int QuestionId { get; set; }

    public Question? Question { get; set; }
}
