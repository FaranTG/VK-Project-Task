using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Shared;

namespace QuizWebApp.Api.Data;

public static class DataExtensions
{
    public static void AddQuizDatabase(this WebApplicationBuilder builder)
    {
        string connectionStringName = "QuizDatabase";
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
        IServiceProvider services = scope.ServiceProvider;

        QuizContext dbContext = services.GetRequiredService<QuizContext>();
        if (dbContext.Users.Any())
        {
            return;
        }

        IPasswordHasher<User> passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();
        IConfiguration configuration = services.GetRequiredService<IConfiguration>();
        SeedQuizUsers(dbContext, passwordHasher, configuration);

        dbContext.SaveChanges();
    }

    private static void SeedQuizUsers(QuizContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        string initialAdminOptionsSectionName = "InitialAdmin";
        InitialAdminOptions initialAdminOptions = configuration.GetSection(initialAdminOptionsSectionName).Get<InitialAdminOptions>()
            ?? throw new InvalidOperationException($"Configuration section with name '{initialAdminOptionsSectionName}' does not exist");
        
        User initialAdmin = new ()
        {
            Name = initialAdminOptions.Name,
            Phone = initialAdminOptions.Phone,
            Email = initialAdminOptions.Email,
            PasswordHash = "somerandomhash",
            Role = nameof(UserRole.Organizer),
            IsApproved = true
        };
        string password = initialAdminOptions.Password;
        initialAdmin.PasswordHash = passwordHasher.HashPassword(initialAdmin, password);

        dbContext.Users.Add(initialAdmin);
    }
}
