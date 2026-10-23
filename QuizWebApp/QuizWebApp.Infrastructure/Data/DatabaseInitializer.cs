using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Validation;
using QuizWebApp.Shared.DTOs.Quiz;
using QuizWebApp.Shared.DTOs.Topic;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly QuizContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly string _seedFolder;

    public DatabaseInitializer
    (
        QuizContext dbContext, 
        IPasswordHasher<User> passwordHasher
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _seedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Seed");
    }

    public void Initialize()
    {
        TryMigrateDatabase();
        TrySeedDatabase();
    }

    private void TryMigrateDatabase()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
        {
            _dbContext.Database.Migrate();
        }
    }

    private void TrySeedDatabase()
    {
        if (_dbContext.Users.Any())
        {
            return;
        }

        SeedUsers();
        SeedTopics();
        SeedQuizzes();

        _dbContext.SaveChanges();
    }

    private void SeedUsers()
    {
        string usersPath = Path.Combine(_seedFolder, "users.json");

        if (!File.Exists(usersPath))
        {
            return;
        }

        string usersJson = File.ReadAllText(usersPath);
        List<JsonElement>? seedElements = JsonSerializer.Deserialize<List<JsonElement>>(usersJson);

        if (seedElements is null || seedElements.Count == 0)
        {
            return;
        }

        foreach (JsonElement element in seedElements)
        {
            UserSaveDTO? dto = JsonSerializer.Deserialize<UserSaveDTO>(element.GetRawText());
            
            if (dto is null)
            {
                continue;
            }

            if (!element.TryGetProperty("Role", out JsonElement roleElement) || string.IsNullOrWhiteSpace(roleElement.GetString()))
            {
                throw new KeyNotFoundException($"Seed Validation Failed! User '{dto.Email}' is missing a valid 'Role' property in users.json.");
            }

            string role = roleElement.GetString()!;

            string? validationError = dto.Validate(role);
            if (validationError is not null)
            {
                throw new ValidationException($"Seed User Domain Validation Failed: {validationError}");
            }

            User newUser = new()
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                PasswordHash = string.Empty,
                Role = role,
                IsApproved = true
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);
            _dbContext.Users.Add(newUser);
        }

        _dbContext.SaveChanges();
    }

    private void SeedTopics()
    {
        string topicsPath = Path.Combine(_seedFolder, "topics.json");
        
        if (!File.Exists(topicsPath))
        {
            return;
        }

        string topicsJson = File.ReadAllText(topicsPath);
        List<TopicSaveDTO>? seedTopics = JsonSerializer.Deserialize<List<TopicSaveDTO>>(topicsJson);
        
        if (seedTopics is null || seedTopics.Count == 0)
        {
            return;
        }

        List<Topic> realTopics = [];

        foreach (TopicSaveDTO dto in seedTopics)
        {
            string? validationError = dto.Validate();
            if (validationError is not null)
            {
                throw new ValidationException($"Seed Topic Domain Validation Failed: {validationError}");
            }

            realTopics.Add(new Topic { Name = dto.Name });
        }
        
        _dbContext.Topics.AddRange(realTopics);
        _dbContext.SaveChanges();
    }

    private void SeedQuizzes()
    {
        string quizzesPath = Path.Combine(_seedFolder, "quizzes.json");
        
        if (!File.Exists(quizzesPath))
        {
            return;
        }

        string quizzesJson = File.ReadAllText(quizzesPath);
        List<QuizSaveDTO>? seedQuizzes = JsonSerializer.Deserialize<List<QuizSaveDTO>>(quizzesJson);
        
        if (seedQuizzes is null)
        {
            return;
        }

        foreach (QuizSaveDTO dto in seedQuizzes)
        {
            string? validationError = dto.Validate();
            if (validationError is not null)
            {
                throw new ValidationException($"Seed Quiz Domain Validation Failed: {validationError}");
            }

            Quiz realQuiz = new()
            {
                Name = dto.Name,
                TopicId = dto.TopicId,
                QuestionsNumber = dto.Questions.Count,
                TimeInMinutes = dto.TimeInMinutes,
                IsActive = dto.IsActive,
                Questions = dto.Questions
                    .Select
                    (
                        q => new Question
                        {
                            Text = q.Text,
                            Options = q.Options
                                .Select
                                (
                                    o => new AnswerOption
                                    {
                                        Text = o.Text,
                                        IsCorrect = o.IsCorrect
                                    }
                                )
                                .ToList()
                        }
                    )
                    .ToList()
            };

            _dbContext.Quizzes.Add(realQuiz);
        }
    }
}
