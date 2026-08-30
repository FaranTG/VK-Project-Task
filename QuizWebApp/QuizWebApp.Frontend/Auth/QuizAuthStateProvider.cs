using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using QuizWebApp.Shared;

namespace QuizWebApp.Frontend.Auth;

public class QuizAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthType = "quiz-auth";
    private const string UserDataKey = "user-data";

    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager _navigationManager;
    private Task<AuthenticationState> _authStateTask;

    public LoggedInUserInfo? User { get; private set; }
    public bool IsLoggedIn => User?.Id > 0;
    public bool IsInitializing { get; private set; } = true;

    public QuizAuthStateProvider(IJSRuntime jsRuntime, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime;
        _navigationManager = navigationManager;
        _authStateTask = CreateAuthStateTask();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;

    public async Task InitializeAsync()
    {
        try
        {
            string? userData = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
            if (string.IsNullOrWhiteSpace(userData))
            {
                RedirectToLogin();
                return;
            }

            LoggedInUserInfo user = LoggedInUserInfo.LoadFromJson(userData);
            if (user is null || user.Id == 0)
            {
                RedirectToLogin();
                return;
            }

            if (IsTokenValid(user.Token))
            {
                await SetLoginAsync(user);
            }
            else
            {
                RedirectToLogin();
            }
        }
        finally
        {
            IsInitializing = false;
        }
    }

    public async Task SetLoginAsync(LoggedInUserInfo user)
    {
        User = user;

        _authStateTask = CreateAuthStateTask();
        NotifyAuthenticationStateChanged(_authStateTask);
        
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, user.ToJson());
    }

    public async Task SetLogoutAsync()
    {
        User = null;

        _authStateTask = CreateAuthStateTask();
        NotifyAuthenticationStateChanged(_authStateTask);

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);
    }

    private void RedirectToLogin()
    {
        _navigationManager.NavigateTo("auth/login");
    }

    private Task<AuthenticationState> CreateAuthStateTask()
    {
        ClaimsIdentity identity = IsLoggedIn ? new (User!.ToClaims(), AuthType) : new ();

        ClaimsPrincipal principal = new (identity);
        AuthenticationState authState = new (principal);

        return Task.FromResult(authState);
    }

    private static bool IsTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        JwtSecurityTokenHandler jwtHandler = new ();
        if (!jwtHandler.CanReadToken(token))
        {
            return false;
        }

        JwtSecurityToken jwt = jwtHandler.ReadJwtToken(token);
        Claim? expirationClaim = jwt.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);
        if (expirationClaim is null)
        {
            return false;
        }

        long expirationTime = long.Parse(expirationClaim.Value);
        DateTime expirationUTCDateTime = DateTimeOffset.FromUnixTimeSeconds(expirationTime).UtcDateTime;

        return expirationUTCDateTime > DateTime.UtcNow;
    }
}
