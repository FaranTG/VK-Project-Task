using Microsoft.AspNetCore.Identity;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Endpoints;
using QuizWebApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.AddQuizDatabase();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.AddJwtAuthentication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateQuizDatabase();
    app.SeedQuizDatabase();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.MapAuthEndpoints();

app.Run();
