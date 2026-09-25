using Microsoft.AspNetCore.Identity;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services;
using QuizWebApp.Api.Services.Interfaces;

namespace QuizWebApp.Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddQuizServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ITopicService, TopicService>()
            .AddScoped<IQuizService, QuizService>()
            .AddScoped<IAttemptService, AttemptService>()
            .AddScoped<IUserService, UserService>();
        
        return serviceCollection;
    }
}