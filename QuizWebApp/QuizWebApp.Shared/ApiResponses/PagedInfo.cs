namespace QuizWebApp.Shared.ApiResponses;

public record PagedInfo<TData>
(
    TData[] Data,
    int TotalCount
);