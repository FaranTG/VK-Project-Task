using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Core.UseCases;

namespace QuizWebApp.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ITopicService, TopicService>()
            .AddScoped<IQuizService, QuizService>()
            .AddScoped<IAttemptService, AttemptService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IOrganizerService, OrganizerService>();
        
        return serviceCollection;
    }
}
