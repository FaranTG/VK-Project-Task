namespace QuizWebApp.Api.Configuration;

public record class JwtOptions
(
    string Secret,
    string Issuer,
    string Audience,
    int ExpireInMinutes
);