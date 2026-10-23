using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Core.UseCases;

namespace QuizWebApp.Core;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ITopicsService, TopicsService>()
            .AddScoped<IQuizzesService, QuizzesService>()
            .AddScoped<IAttemptsService, AttemptsService>()
            .AddScoped<IUsersService, UsersService>()
            .AddScoped<IOrganizerService, OrganizerService>();
        
        return serviceCollection;
    }
}
