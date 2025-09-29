using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddModeToQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimitSeconds",
                table: "Questions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "TimeLimitSeconds",
                table: "Questions");
        }
    }
}
