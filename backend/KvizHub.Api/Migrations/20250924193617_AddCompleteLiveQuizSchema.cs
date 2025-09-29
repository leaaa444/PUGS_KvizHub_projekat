using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteLiveQuizSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionIndex",
                table: "GameRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentQuestionStartTime",
                table: "GameRooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HostDisconnectedAt",
                table: "GameRooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GameRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LiveQuizParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    GameRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveQuizParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveQuizParticipants_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveQuizParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerTimeMilliseconds = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsAwarded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantAnswers_LiveQuizParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "LiveQuizParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantSelectedOptions",
                columns: table => new
                {
                    ParticipantAnswerId = table.Column<int>(type: "int", nullable: false),
                    AnswerOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantSelectedOptions", x => new { x.ParticipantAnswerId, x.AnswerOptionId });
                    table.ForeignKey(
                        name: "FK_ParticipantSelectedOptions_AnswerOptions_AnswerOptionId",
                        column: x => x.AnswerOptionId,
                        principalTable: "AnswerOptions",
                        principalColumn: "AnswerOptionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantSelectedOptions_ParticipantAnswers_ParticipantAnswerId",
                        column: x => x.ParticipantAnswerId,
                        principalTable: "ParticipantAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_QuizId",
                table: "GameRooms",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizParticipants_GameRoomId",
                table: "LiveQuizParticipants",
                column: "GameRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveQuizParticipants_UserId",
                table: "LiveQuizParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAnswers_ParticipantId",
                table: "ParticipantAnswers",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAnswers_QuestionId",
                table: "ParticipantAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantSelectedOptions_AnswerOptionId",
                table: "ParticipantSelectedOptions",
                column: "AnswerOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRooms_Quizzes_QuizId",
                table: "GameRooms",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRooms_Quizzes_QuizId",
                table: "GameRooms");

            migrationBuilder.DropTable(
                name: "ParticipantSelectedOptions");

            migrationBuilder.DropTable(
                name: "ParticipantAnswers");

            migrationBuilder.DropTable(
                name: "LiveQuizParticipants");

            migrationBuilder.DropIndex(
                name: "IX_GameRooms_QuizId",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionIndex",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionStartTime",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "HostDisconnectedAt",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GameRooms");

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameRoomId = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameRoomId",
                table: "Players",
                column: "GameRoomId");
        }
    }
}
