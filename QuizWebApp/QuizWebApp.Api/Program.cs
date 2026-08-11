using Microsoft.AspNetCore.Identity;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.AddQuizDatabase();

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

app.Run();
