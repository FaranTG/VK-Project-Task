using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuizWebApp.Core.Configuration;

namespace QuizWebApp.Api.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string jwtOptionsSectionName = "Jwt";
        JwtOptions jwtOptions = configuration.GetSection(jwtOptionsSectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException($"Configuration section with name '{jwtOptionsSectionName}' does not exist");
        
        services
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
        
        return services;
    }

    public static IServiceCollection AddQuizCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(
            options =>
            {
                options.AddDefaultPolicy
                (
                    policy =>
                    {
                        string allowedOriginsName = "AllowedOrigins";
                        string allowedOrigins = configuration.GetValue<string>(allowedOriginsName)
                            ?? throw new InvalidOperationException($"Configuration value with name '{allowedOriginsName}' does not exist");

                        string[] allowedOriginsArray = allowedOrigins.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        policy
                            .WithOrigins(allowedOriginsArray)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            } 
        );

        return services;
    }
}
