using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.Shared.DTOs.Common;

public record PaginationDTO
(
    [Range(1, int.MaxValue)]
    int PageNumber,

    [Range(1, 100)]
    int PageSize
);