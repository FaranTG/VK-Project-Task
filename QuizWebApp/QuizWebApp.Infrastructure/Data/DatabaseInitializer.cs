using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuizWebApp.Core.Configuration;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Validation;
using QuizWebApp.Shared.DTOs.Quiz;
using QuizWebApp.Shared.DTOs.Topic;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly QuizContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly InitialAdminOptions _adminOptions;

    public DatabaseInitializer
    (
        QuizContext dbContext, 
        IPasswordHasher<User> passwordHasher, 
        IOptions<InitialAdminOptions> adminOptions
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _adminOptions = adminOptions.Value;
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

        User initialAdmin = new ()
        {
            Name = _adminOptions.Name,
            Phone = _adminOptions.Phone,
            Email = _adminOptions.Email,
            PasswordHash = string.Empty,
            Role = nameof(UserRole.Organizer),
            IsApproved = true
        };
        
        initialAdmin.PasswordHash = _passwordHasher
            .HashPassword(initialAdmin, _adminOptions.Password);

        _dbContext.Users.Add(initialAdmin);
        
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        SeedTopics(baseDir);
        SeedQuizzes(baseDir);

        _dbContext.SaveChanges();
    }

    private void SeedTopics(string baseDir)
    {
        string topicsPath = Path.Combine(baseDir, "Data", "Seed", "topics.json");
        
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

            realTopics.Add(new Topic 
            { 
                Name = dto.Name 
            });
        }
        
        _dbContext.Topics.AddRange(realTopics);
        _dbContext.SaveChanges();
    }

    private void SeedQuizzes(string baseDir)
    {
        string quizzesPath = Path.Combine(baseDir, "Data", "Seed", "quizzes.json");
        
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
