using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Core.Entities;

public class Topic
{
    public int Id { get; set; }

    [Required][StringLength(20)]
    public required string Name { get; set; }
}
