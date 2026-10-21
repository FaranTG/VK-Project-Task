using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizWebApp.Core.Configuration;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Core.UseCases;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid username or password.";
    private const string DuplicateMessage = "Email already exists.";
    private const string ApprovalMessage = "Your account is not approved yet.";

    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUsersRepository usersRepository, IPasswordHasher<User> passwordHasher, IOptions<JwtOptions> jwtOptions)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<QuizApiResponse<LoggedInUserInfo>> LoginAsync(UserLoginDTO loginData)
    {
        try
        {
            User? user = await _usersRepository
                .GetByEmailAsync(loginData.Username);

            if (user is null)
            {
                return QuizApiResponse<LoggedInUserInfo>.Fail(InvalidCredentialsMessage);
            }

            if (!user.IsApproved)
            {
                return QuizApiResponse<LoggedInUserInfo>.Fail(ApprovalMessage);
            }

            PasswordVerificationResult passwordCheckResult = _passwordHasher
                .VerifyHashedPassword(user, user.PasswordHash, loginData.Password);
            
            if (passwordCheckResult == PasswordVerificationResult.Failed)
            {
                return QuizApiResponse<LoggedInUserInfo>.Fail(InvalidCredentialsMessage);
            }

            return QuizApiResponse<LoggedInUserInfo>.Success(CreateLoggedInUserInfo(user));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<LoggedInUserInfo>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> RegisterAsync(UserSaveDTO userData)
    {
        try
        {
            bool isEmailDuplicate = await _usersRepository
                .ExistsByEmailAsync(userData.Email);

            if (isEmailDuplicate)
            {
                return QuizApiResponse.Fail(DuplicateMessage);
            }

            await _usersRepository
                .AddAsync(CreateUser(userData));

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private User CreateUser(UserSaveDTO userData)
    {
        User user = new ()
        {
            Name = userData.Name,
            Phone = userData.Phone,
            Email = userData.Email,
            PasswordHash = string.Empty,
            Role = nameof(UserRole.Participant)
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userData.Password);

        return user;
    }

    private LoggedInUserInfo CreateLoggedInUserInfo(User user)
        => new (user.Id, user.Name, user.Role, GenerateJwtToken(user));

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
            notBefore: null, 
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireInMinutes),
            signingCredentials: signingCredentials
        );

        string packedJwtToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return packedJwtToken;
    }
}