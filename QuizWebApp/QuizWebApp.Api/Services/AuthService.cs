using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared.DTOs;

namespace QuizWebApp.Api.Services;

public class AuthService : IAuthService
{
    private readonly QuizContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(QuizContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
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

        string jwt = GenerateJWTToken(user);
        return new AuthResponseDTO(jwt, null);
    }

    private string GenerateJWTToken(User user)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        ];

        string secretKey = _configuration.GetValue<string>("JWT:Secret")
            ?? throw new InvalidOperationException($"Value with name 'JWT:Secret' does not exist");
        SymmetricSecurityKey symmetricKey = new(System.Text.Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials signingCredentials = new(symmetricKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new
        (
            issuer: _configuration.GetValue<string>("JWT:Issuer"), 
            audience: _configuration.GetValue<string>("JWT:Audience"), 
            claims: claims,
            notBefore: DateTime.UtcNow, 
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes")),
            signingCredentials: signingCredentials
        );

        string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return token;
    }
}
