using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared;

namespace QuizWebApp.Api.Data;

public static class DataExtensions
{
    public static void AddQuizDatabase(this WebApplicationBuilder builder)
    {
        string connectionStringName = "Quiz";
        string connectionString = builder.Configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string with name '{connectionStringName}' does not exist");

        builder.Services.AddDbContext<QuizContext>(
            options => options.UseNpgsql(connectionString)
        );
    }

    public static void MigrateQuizDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        QuizContext dbContext = scope.ServiceProvider.GetRequiredService<QuizContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }

    public static void SeedQuizDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        QuizContext dbContext = scope.ServiceProvider.GetRequiredService<QuizContext>();
        if (dbContext.Users.Any())
        {
            return;
        }

        IPasswordHasher<User> passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        SeedQuizUsers(dbContext, passwordHasher);

        dbContext.SaveChanges();
    }

    private static void SeedQuizUsers(QuizContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        User initialAdmin = new ()
        {
            Name = "InitialAdmin",
            Phone = "111111111111",
            Email = "initialadmin@quiz.ru",
            PasswordHash = "somerandomhash",
            Role = nameof(UserRole.Organizer),
            IsApproved = true
        };
        string password = "12345";
        initialAdmin.PasswordHash = passwordHasher.HashPassword(initialAdmin, password);

        dbContext.Users.Add(initialAdmin);
    }
}
