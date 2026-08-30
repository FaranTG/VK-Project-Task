namespace QuizWebApp.Shared;

public record QuizApiResponse<T>
(
    T? Data,
    string? ErrorMessage,
    bool IsSuccess
)
{
    public static QuizApiResponse<T> Success(T Data) => new (Data, null, true);

    public static QuizApiResponse<T> Fail(string errorMessage) => new (default, errorMessage, false);
}