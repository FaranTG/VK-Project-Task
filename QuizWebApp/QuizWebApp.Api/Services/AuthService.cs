using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared;
using QuizWebApp.Shared.DTOs;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid username or password.";

    private readonly QuizContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(QuizContext dbContext, IPasswordHasher<User> passwordHasher, IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginData)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == loginData.Username);

        if (user is null)
        {
            return new AuthResponseDTO(null, InvalidCredentialsMessage);
        }

        if (!user.IsApproved)
        {
            return new AuthResponseDTO(null, "Your account is not approved yet.");
        }

        PasswordVerificationResult passwordCheckResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginData.Password);
        
        if (passwordCheckResult == PasswordVerificationResult.Failed)
        {
            return new AuthResponseDTO(null, InvalidCredentialsMessage);
        }

        string jwtToken = GenerateJwtToken(user);
        LoggedInUser loggedInUser = new 
        (
            user.Id,
            user.Name,
            user.Role,
            jwtToken
        );

        return new AuthResponseDTO(loggedInUser, null);
    }

    public async Task<QuizApiResponse<UserInfoDTO>> RegisterAsync(UserSaveDTO userData)
    {
        if (await _dbContext.Users.AnyAsync(user => user.Email == userData.Email))
        {
            return QuizApiResponse<UserInfoDTO>.Fail("Email already exists.");
        }

        User user = new ()
        {
            Name = userData.Name,
            Phone = userData.Phone,
            Email = userData.Email,
            PasswordHash = string.Empty,
            Role = UserRole.Participant.ToString()
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userData.Password);

        _dbContext.Users.Add(user);
        try
        {
            await _dbContext.SaveChangesAsync();

            UserInfoDTO userInfo = new
            (
                user.Id,
                user.Name,
                user.Phone,
                user.Email,
                user.Role,
                user.IsApproved
            );

            return QuizApiResponse<UserInfoDTO>.Success(userInfo);
        }
        catch (Exception exception)
        {
            return QuizApiResponse<UserInfoDTO>.Fail(exception.Message);
        }
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
