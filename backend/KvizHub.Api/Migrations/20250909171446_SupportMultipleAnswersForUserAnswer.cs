using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class SupportMultipleAnswersForUserAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedAnswerOptionID",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_SelectedAnswerOptionID",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "SelectedAnswerOptionID",
                table: "UserAnswers");

            migrationBuilder.CreateTable(
                name: "UserAnswerSelectedOptions",
                columns: table => new
                {
                    UserAnswerId = table.Column<int>(type: "int", nullable: false),
                    AnswerOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswerSelectedOptions", x => new { x.UserAnswerId, x.AnswerOptionId });
                    table.ForeignKey(
                        name: "FK_UserAnswerSelectedOptions_AnswerOptions_AnswerOptionId",
                        column: x => x.AnswerOptionId,
                        principalTable: "AnswerOptions",
                        principalColumn: "AnswerOptionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnswerSelectedOptions_UserAnswers_UserAnswerId",
                        column: x => x.UserAnswerId,
                        principalTable: "UserAnswers",
                        principalColumn: "UserAnswerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerSelectedOptions_AnswerOptionId",
                table: "UserAnswerSelectedOptions",
                column: "AnswerOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnswerSelectedOptions");

            migrationBuilder.AddColumn<int>(
                name: "SelectedAnswerOptionID",
                table: "UserAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_SelectedAnswerOptionID",
                table: "UserAnswers",
                column: "SelectedAnswerOptionID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedAnswerOptionID",
                table: "UserAnswers",
                column: "SelectedAnswerOptionID",
                principalTable: "AnswerOptions",
                principalColumn: "AnswerOptionID");
        }
    }
}
