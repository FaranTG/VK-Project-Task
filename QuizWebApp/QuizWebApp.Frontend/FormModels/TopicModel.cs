using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Frontend.FormModels;

public class TopicModel
{
    [Required][StringLength(30)]
    public required string Name { get; set; }
}
