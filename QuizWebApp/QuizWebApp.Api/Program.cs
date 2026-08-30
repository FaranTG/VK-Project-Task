using Microsoft.AspNetCore.Identity;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Endpoints;
using QuizWebApp.Api.Services;
using QuizWebApp.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.AddQuizDatabase();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.AddJwtAuthentication();
builder.AddQuizCors();

builder.Services.AddAuthorization();

builder.AddQuizSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateQuizDatabase();
    app.SeedQuizDatabase();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseCors();

app.UseAuthorization();

app
    .MapAuthEndpoints()
    .MapTopicEndpoints()
    .MapQuizEndpoints();

app.Run();
