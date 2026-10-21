using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Infrastructure.Data;
using QuizWebApp.Infrastructure.Repositories;
using QuizWebApp.Infrastructure.Workers;

namespace QuizWebApp.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddWorkers();

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringName = "QuizDatabase";
        string connectionString = configuration
            .GetConnectionString(connectionStringName) 
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        return services
            .AddDbContext<QuizContext>(options => options.UseNpgsql(connectionString))
            .AddScoped<DatabaseInitializer>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
        => services
            .AddScoped<IOrganizerRepository, OrganizerRepository>()
            .AddScoped<ITopicsRepository, TopicsRepository>()
            .AddScoped<IUsersRepository, UsersRepository>()
            .AddScoped<IAttemptsRepository, AttemptsRepository>()
            .AddScoped<IQuizzesRepository, QuizzesRepository>();

    private static IServiceCollection AddWorkers(this IServiceCollection services)
        => services
            .AddHostedService<QuizExpirationWorker>();
}
