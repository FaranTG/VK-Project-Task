namespace QuizWebApp.Shared.Interfaces;

public interface IAppState
{
    event Action? OnToggleLoader;

    string? LoadingText { get; }
    bool IsLoading => !string.IsNullOrWhiteSpace(LoadingText);

    void ShowLoader(string loadingText);
    
    void HideLoader();
}
