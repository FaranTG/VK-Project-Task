using QuizWebApp.Shared.Interfaces;

namespace QuizWebApp.Frontend;

public class AppState : IAppState
{
    public event Action<string>? OnShowError;
    public event Action? OnToggleLoader;

    public string? LoadingText { get; private set; }

    public void ShowError(string errorText)
     => OnShowError?.Invoke(errorText);

    public void ShowLoader(string loadingText)
    {
        LoadingText = loadingText;
        OnToggleLoader?.Invoke();
    }

    public void HideLoader()
    {
        LoadingText = null;
        OnToggleLoader?.Invoke();
    }
}
