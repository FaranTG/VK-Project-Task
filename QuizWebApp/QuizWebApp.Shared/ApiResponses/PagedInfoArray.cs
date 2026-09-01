namespace QuizWebApp.Shared.ApiResponses;

public record PagedInfoArray<TInfoDTO>
(
    TInfoDTO[] Records,
    int TotalCount
);