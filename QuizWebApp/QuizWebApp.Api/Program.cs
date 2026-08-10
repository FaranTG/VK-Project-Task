using QuizWebApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddQuizDatabase();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
