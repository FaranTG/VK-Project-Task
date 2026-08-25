using System.Net.Http.Json;
using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend.Clients;

public class QuizzesClient(HttpClient httpClient)
{
    private const string ApiRoute = "/api/quizzes";

    public async Task<QuizInfoDTO[]> GetQuizzesAsync()
        => await httpClient.GetFromJsonAsync<QuizInfoDTO[]>(ApiRoute)
        ?? throw new InvalidOperationException("Could not find quizzes list.");
    
    public async Task<QuizInfoDTO> GetQuizByIdAsync(Guid id)
        => await httpClient.GetFromJsonAsync<QuizInfoDTO>($"{ApiRoute}/{id}")
        ?? throw new InvalidOperationException("Could not find quiz.");
    
    public async Task AddQuizAsync(QuizSaveDTO newQuiz)
        => await httpClient.PostAsJsonAsync(ApiRoute, newQuiz);

    public async Task UpdateQuizAsync(Guid id, QuizSaveDTO newQuiz)
        => await httpClient.PutAsJsonAsync($"{ApiRoute}/{id}", newQuiz);

    public async Task DeleteQuizAsync(Guid id)
        => await httpClient.DeleteAsync($"{ApiRoute}/{id}");
}
