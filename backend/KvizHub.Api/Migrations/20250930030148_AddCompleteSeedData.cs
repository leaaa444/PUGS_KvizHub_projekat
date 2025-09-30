using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "AnswerOptionID", "IsCorrect", "QuestionID", "Text" },
                values: new object[] { 212, false, 83, "Grčka" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 212);
        }
    }
}
