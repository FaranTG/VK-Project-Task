using QuizWebApp.Api.Extensions;
using QuizWebApp.Core;
using QuizWebApp.Core.Configuration;
using QuizWebApp.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"))
    .Configure<InitialAdminOptions>(builder.Configuration.GetSection("InitialAdmin"))
    .AddValidation()
    .AddCore()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddQuizCors(builder.Configuration)
    .AddAuthorization()
    .AddSwagger();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.InitializeDatabase();

    app
        .UseSwagger()
        .UseSwaggerUI();
}

app
    .UseAuthentication()
    .UseCors()
    .UseAuthorization();

app.MapEndpoints();

app.Run();
