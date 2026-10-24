using System.ComponentModel.DataAnnotations;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Shared.DTOs.Filter;

public record AttemptFilterDTO
(
    [StringLength(100)]
    string? SearchFilter,

    AttemptStatus? StatusFilter
);