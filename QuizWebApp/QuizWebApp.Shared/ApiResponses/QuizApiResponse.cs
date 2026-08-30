namespace QuizWebApp.Shared.ApiResponses;

public record QuizApiResponse
(
    string? ErrorMessage
)
{
    public bool IsSuccess => ErrorMessage is null;
    public bool IsFailure => !IsSuccess;

    public static QuizApiResponse Success() => new (ErrorMessage: null);

    public static QuizApiResponse Fail(string errorMessage) => new (errorMessage);
}

public record QuizApiResponse<T>
(
    T? Data,
    string? ErrorMessage
)
{
    public bool IsSuccess => ErrorMessage is null;
    public bool IsFailure => !IsSuccess;

    public static QuizApiResponse<T> Success(T Data) => new (Data, null);

    public static QuizApiResponse<T> Fail(string errorMessage) => new (default, errorMessage);
}