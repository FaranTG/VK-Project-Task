namespace QuizWebApp.Shared.ApiResponses;

public record PagedInfo<TInfoDTO>
(
    TInfoDTO[] DTOs,
    int TotalCount
);