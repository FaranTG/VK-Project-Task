using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Topic;

public record class TopicSaveDTO
(
    [Required][StringLength(30)]
    string Name
);