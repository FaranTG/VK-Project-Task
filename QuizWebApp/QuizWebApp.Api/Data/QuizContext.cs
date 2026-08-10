using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data.Models;

namespace QuizWebApp.Api.Data;

public class QuizContext(DbContextOptions<QuizContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<Quiz> Quizzes => Set<Quiz>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();

    public DbSet<Attempt> Attempts => Set<Attempt>();
}