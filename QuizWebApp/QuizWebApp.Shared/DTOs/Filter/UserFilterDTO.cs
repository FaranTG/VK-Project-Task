using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Filter;

public record UserFilterDTO
(
    [StringLength(100)]
    string? SearchFilter,

    bool? IsApprovedFilter
);