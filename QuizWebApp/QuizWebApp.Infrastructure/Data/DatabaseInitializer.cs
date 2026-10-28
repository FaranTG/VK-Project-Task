using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Validation;
using QuizWebApp.Shared.DTOs.Quiz;
using QuizWebApp.Shared.DTOs.Topic;
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

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

        SeedAttempts();

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
            ProcessUserElement(element);
        }

        _dbContext.SaveChanges();
    }

    private void ProcessUserElement(JsonElement element)
    {
        UserSaveDTO? dto = JsonSerializer.Deserialize<UserSaveDTO>(element.GetRawText());
        
        if (dto is null)
        {
            return;
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

        User newUser = CreateUser(dto, role);

        _dbContext.Users.Add(newUser);
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

            Quiz realQuiz = CreateQuiz(dto);

            _dbContext.Quizzes.Add(realQuiz);
        }
    }

    private void SeedAttempts()
    {
        List<User> participants = _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == "Participant")
            .ToList();

        if (participants.Count == 0) return;

        List<Quiz> quizzes = _dbContext.Quizzes
            .AsNoTracking()
            .ToList();

        if (quizzes.Count == 0) return;

        Random random = new();
        DateTimeOffset baseDate = DateTimeOffset.UtcNow;

        foreach (User participant in participants)
        {
            foreach (Quiz quiz in quizzes)
            {
                SeedThreeAttempts(participant, quiz, random, baseDate);
            }
        }
    }

    private void SeedThreeAttempts(User participant, Quiz quiz, Random random, DateTimeOffset baseDate)
    {
        SeedCompletedAttempt(participant, quiz, random, baseDate);
        SeedAutoSubmittedAttempt(participant, quiz, random, baseDate);
        SeedStartedAttempt(participant, quiz, random, baseDate);
    }

    private void SeedCompletedAttempt(User participant, Quiz quiz, Random random, DateTimeOffset baseDate)
    {
        int maxQuestions = quiz.QuestionsNumber;
        
        int daysAgoCompleted = random.Next(3, 8);
        DateTimeOffset startTime = baseDate.AddDays(-daysAgoCompleted).AddHours(random.Next(-5, 5));
        
        int durationMinutes = random.Next(1, quiz.TimeInMinutes + 1);
        DateTimeOffset endTime = startTime.AddMinutes(durationMinutes);
        int score = random.Next(0, maxQuestions + 1);

        CreateAttempt(participant.Id, quiz.Id, AttemptStatus.Completed, score, startTime, endTime);
    }

    private void SeedAutoSubmittedAttempt(User participant, Quiz quiz, Random random, DateTimeOffset baseDate)
    {
        int maxQuestions = quiz.QuestionsNumber;
        
        int daysAgoAuto = random.Next(1, 3);
        DateTimeOffset startTime = baseDate.AddDays(-daysAgoAuto).AddHours(random.Next(-5, 5));
        
        DateTimeOffset endTime = startTime.AddMinutes(quiz.TimeInMinutes);
        int score = random.Next(0, maxQuestions + 1);

        CreateAttempt(participant.Id, quiz.Id, AttemptStatus.AutoSubmitted, score, startTime, endTime);
    }

    private void SeedStartedAttempt(User participant, Quiz quiz, Random random, DateTimeOffset baseDate)
    {
        int maxQuestions = quiz.QuestionsNumber;
        
        int minutesAgoStarted = random.Next(0, quiz.TimeInMinutes);
        int secondsAgoStarted = random.Next(0, 60);
        
        DateTimeOffset startTime = baseDate
            .AddMinutes(-minutesAgoStarted)
            .AddSeconds(-secondsAgoStarted);
            
        int score = random.Next(0, maxQuestions);

        CreateAttempt(participant.Id, quiz.Id, AttemptStatus.Started, score, startTime, null);
    }

    private User CreateUser(UserSaveDTO dto, string role)
    {
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
        
        return newUser;
    }

    private Quiz CreateQuiz(QuizSaveDTO dto)
    {
        return new Quiz
        {
            Name = dto.Name,
            TopicId = dto.TopicId,
            QuestionsNumber = dto.Questions.Count,
            TimeInMinutes = dto.TimeInMinutes,
            IsActive = dto.IsActive,
            Questions = dto.Questions
                .Select(q => new Question
                {
                    Text = q.Text,
                    Options = q.Options
                        .Select(o => new AnswerOption
                        {
                            Text = o.Text,
                            IsCorrect = o.IsCorrect
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private void CreateAttempt(
        int participantId, 
        Guid quizId, 
        AttemptStatus status, 
        int score, 
        DateTimeOffset startTime, 
        DateTimeOffset? endTime)
    {
        Attempt attempt = new Attempt
        {
            ParticipantId = participantId,
            QuizId = quizId,
            Status = status.ToString(),
            Score = score,
            StartTime = startTime
        };

        if (endTime.HasValue)
        {
            attempt.EndTime = endTime.Value;
        }

        _dbContext.Attempts.Add(attempt);
    }
}
