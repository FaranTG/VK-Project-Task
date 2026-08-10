using Microsoft.EntityFrameworkCore;

namespace QuizWebApp.Api.Data;

public static class DataExtensions
{
    public static void AddQuizDatabase(this WebApplicationBuilder builder)
    {
        string connectionStringName = "Quiz";
        string connectionString = builder.Configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string with name '{connectionStringName}' does not exists");

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
}
