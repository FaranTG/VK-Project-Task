using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Filter;

public record TopicFilterDTO
(
    [StringLength(100)]
    string? SearchFilter
);