using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Filter;

public record QuizFilterDTO
(
    [StringLength(100)]
    string? SearchFilter,

    [Range(0, int.MaxValue)]
    int TopicFilterId
);