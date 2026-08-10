namespace QuizWebApp.Api.Data.Models;

public class Quiz
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public int TopicId { get; set; }

    public Topic? Topic { get; set; }

    public int QuestionsNumber { get; set; }

    public int Time { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Question>? Questions { get; set; }
}
