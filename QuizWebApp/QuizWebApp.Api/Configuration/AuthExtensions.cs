using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace QuizWebApp.Api.Configuration;

public static class AuthExtensions
{
    public static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        string jwtOptionsSectionName = "Jwt";
        JwtOptions jwtOptions = builder.Configuration.GetSection(jwtOptionsSectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException($"Configuration section with name '{jwtOptionsSectionName}' does not exist");
        
        builder.Services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                options =>
                {
                    SymmetricSecurityKey signingSymmetricKey = new(System.Text.Encoding.UTF8.GetBytes(jwtOptions.Secret));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = signingSymmetricKey,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                }
            );
    }
}
