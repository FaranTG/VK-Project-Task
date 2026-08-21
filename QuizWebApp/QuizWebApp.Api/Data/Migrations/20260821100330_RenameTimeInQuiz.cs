using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizWebApp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTimeInQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Quizzes",
                newName: "TimeInMinutes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeInMinutes",
                table: "Quizzes",
                newName: "Time");
        }
    }
}
