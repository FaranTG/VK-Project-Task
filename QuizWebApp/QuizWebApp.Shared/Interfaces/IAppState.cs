namespace QuizWebApp.Shared.Interfaces;

public interface IAppState
{
    event Action<string>? OnShowError;
    event Action? OnToggleLoader;

    string? LoadingText { get; }
    bool IsLoading => !string.IsNullOrWhiteSpace(LoadingText);

    void ShowError(string errorText);
    void ShowLoader(string loadingText);
    void HideLoader();
}
