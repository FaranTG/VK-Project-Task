using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Api.Services;

public class AuthService : IAuthService
{
    private readonly QuizContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(QuizContext context, IPasswordHasher<User> passwordHasher, IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO data)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == data.Username);

        const string invalidCredentialsMessage = "Invalid username or password.";

        if (user is null)
        {
            return new AuthResponseDTO(null, invalidCredentialsMessage);
        }

        PasswordVerificationResult passwordCheckResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, data.Password);
        
        if (passwordCheckResult == PasswordVerificationResult.Failed)
        {
            return new AuthResponseDTO(null, invalidCredentialsMessage);
        }

        string jwtToken = GenerateJwtToken(user);
        return new AuthResponseDTO(jwtToken, null);
    }

    private string GenerateJwtToken(User user)
    {
        Claim[] claims =
        [
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.Name),
            new (ClaimTypes.Role, user.Role)
        ];

        SymmetricSecurityKey signingSymmetricKey = new (System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        SigningCredentials signingCredentials = new (signingSymmetricKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new
        (
            issuer: _jwtOptions.Issuer, 
            audience: _jwtOptions.Audience, 
            claims: claims,
            notBefore: DateTime.UtcNow, 
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireInMinutes),
            signingCredentials: signingCredentials
        );

        string packedJwtToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return packedJwtToken;
    }
}
