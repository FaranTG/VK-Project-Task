using System.Security.Claims;
using System.Text.Json;

namespace QuizWebApp.Shared;

public record LoggedInUserInfo
(
    int Id,
    string Name,
    string Role,
    string Token
)
{
    public string ToJson() => JsonSerializer.Serialize(this);

    public Claim[] ToClaims() =>
    [
        new (ClaimTypes.NameIdentifier, Id.ToString()),
        new (ClaimTypes.Name, Name),
        new (ClaimTypes.Role, Role),
        new (nameof(Token), Token)
    ];

    public static LoggedInUserInfo LoadFromJson(string json) => JsonSerializer.Deserialize<LoggedInUserInfo>(json)
        ?? throw new InvalidOperationException("Cannot convert the parameter from json to LoggedInUser.");
}