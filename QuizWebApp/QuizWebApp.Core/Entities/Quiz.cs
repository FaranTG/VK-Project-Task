using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Core.Entities;

public class Quiz
{
    public Guid Id { get; set; }

    [Required][StringLength(25)]
    public required string Name { get; set; }

    public int TopicId { get; set; }

    public Topic? Topic { get; set; }

    [Range(1, 20)]
    public int QuestionsNumber { get; set; }

    [Range(1, 120)]
    public int TimeInMinutes { get; set; }

    public bool IsActive { get; set; }

    public required List<Question> Questions { get; set; }
}
