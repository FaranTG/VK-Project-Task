using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using QuizWebApp.Shared;

namespace QuizWebApp.Frontend.Auth;

public class QuizAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthType = "quiz-auth";
    private const string UserDataKey = "user-data";

    private readonly IJSRuntime _jsRuntime;
    private Task<AuthenticationState> _authStateTask;

    public LoggedInUser? User { get; private set; }
    public bool IsLoggedIn => User?.Id > 0;
    public bool IsInitializing { get; private set; } = true;

    public QuizAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        SetAuthStateTask();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authStateTask;

    public async Task InitializeAsync()
    {
        try
        {
            string? userData = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserDataKey);
            if (string.IsNullOrWhiteSpace(userData))
            {
                return;
            }

            LoggedInUser user = LoggedInUser.LoadFromJson(userData);
            if (user.Id == 0)
            {
                return;
            }

            await SetLoginAsync(user);
        }
        finally
        {
            IsInitializing = false;
        }
    }

    public async Task SetLoginAsync(LoggedInUser user)
    {
        User = user;

        SetAuthStateTask();
        NotifyAuthenticationStateChanged(_authStateTask);
        
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserDataKey, user.ToJson());
    }

    public async Task SetLogoutAsync()
    {
        User = null;

        SetAuthStateTask();
        NotifyAuthenticationStateChanged(_authStateTask);

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserDataKey);
    }

    private void SetAuthStateTask()
    {
        ClaimsIdentity identity = IsLoggedIn ? new (User!.ToClaims(), AuthType) : new ();

        ClaimsPrincipal principal = new (identity);
        AuthenticationState authState = new (principal);

        _authStateTask = Task.FromResult(authState);
    }
}
