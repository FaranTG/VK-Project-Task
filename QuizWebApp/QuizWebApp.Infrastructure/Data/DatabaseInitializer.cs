using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuizWebApp.Core.Configuration;
using QuizWebApp.Core.Entities;
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
        
        _dbContext.SaveChanges();
    }
}
