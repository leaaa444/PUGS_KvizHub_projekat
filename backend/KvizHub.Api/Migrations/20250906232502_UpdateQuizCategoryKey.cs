using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizCategoryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizCategoryID",
                table: "QuizCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizCategoryID",
                table: "QuizCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
