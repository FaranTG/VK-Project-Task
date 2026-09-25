using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.Services.AddQuizServices();

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

app.MapAllQuizEndpoints();

app.Run();
