using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteSeedData1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 210,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { false, 83, "Egipat" });

            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 211,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { true, 83, "Jordan" });

            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 212,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { true, 85, "Tačno" });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "AnswerOptionID", "IsCorrect", "QuestionID", "Text" },
                values: new object[,]
                {
                    { 204, false, 79, "Grčka" },
                    { 205, true, 81, "Tačno" },
                    { 206, false, 81, "Netačno" },
                    { 207, true, 82, "Brazil" },
                    { 208, false, 82, "Meksiko" },
                    { 209, true, 82, "Argentina" },
                    { 213, false, 85, "Netačno" },
                    { 215, false, 87, "Do Not Repeat Yourself" },
                    { 216, true, 87, "Don't Repeat Yourself" },
                    { 217, false, 87, "Drive Route Yellow" },
                    { 218, true, 88, "GET" },
                    { 219, true, 88, "POST" },
                    { 220, false, 88, "FETCH" },
                    { 221, true, 88, "PUT" },
                    { 222, false, 89, "FirstOrDefault()" },
                    { 223, true, 89, "AsNoTracking()" },
                    { 224, false, 89, "Include()" }
                });

            migrationBuilder.InsertData(
                table: "GameRooms",
                columns: new[] { "Id", "CurrentQuestionIndex", "CurrentQuestionStartTime", "FinishedAt", "HostDisconnectedAt", "HostUsername", "QuizId", "RoomCode", "Status" },
                values: new object[] { 1210, 0, null, new DateTime(2025, 9, 29, 14, 30, 0, 0, DateTimeKind.Utc), null, "djordje", 16, "SEED01", 2 });

            migrationBuilder.InsertData(
                table: "LiveQuizParticipants",
                columns: new[] { "Id", "DisconnectedAt", "GameRoomId", "Score", "UserId" },
                values: new object[,]
                {
                    { 1222, null, 1210, 960, 2 },
                    { 1223, null, 1210, 610, 6 },
                    { 1224, null, 1210, 300, 7 },
                    { 1225, null, 1210, 110, 8 }
                });

            migrationBuilder.InsertData(
                table: "ParticipantAnswers",
                columns: new[] { "Id", "AnswerTimeMilliseconds", "IsCorrect", "ParticipantId", "PointsAwarded", "QuestionId", "SubmittedAnswer" },
                values: new object[,]
                {
                    { 194, 2500, true, 1222, 175, 71, "Tačno" },
                    { 195, 4000, true, 1222, 220, 72, "Mars" },
                    { 196, 6000, true, 1222, 280, 73, "-6,-7" },
                    { 197, 5000, false, 1222, 0, 74, "Osaka" },
                    { 198, 7000, true, 1222, 210, 75, "-11" },
                    { 199, 1500, true, 1222, 75, 76, "Netačno" },
                    { 200, 4000, true, 1223, 160, 71, "Tačno" },
                    { 201, 8000, true, 1223, 192, 72, "Mars" },
                    { 202, 9000, false, 1223, 0, 73, "-6,-8" },
                    { 203, 6000, true, 1223, 140, 74, "Tokio" },
                    { 204, 11000, true, 1223, 172, 75, "-11" },
                    { 205, 3000, false, 1223, 0, 76, "Tačno" },
                    { 206, 5000, false, 1224, 0, 71, "Netačno" },
                    { 207, 12000, true, 1224, 165, 72, "Mars" },
                    { 208, 10000, true, 1224, 116, 74, "Tokio" },
                    { 209, 6000, false, 1224, 0, 76, "Tačno" },
                    { 210, 9000, true, 1225, 110, 71, "Tačno" },
                    { 211, 14000, false, 1225, 0, 72, "Jupiter" }
                });

            migrationBuilder.InsertData(
                table: "ParticipantSelectedOptions",
                columns: new[] { "AnswerOptionId", "ParticipantAnswerId" },
                values: new object[,]
                {
                    { 194, 196 },
                    { 195, 196 },
                    { 194, 202 },
                    { 196, 202 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "ParticipantSelectedOptions",
                keyColumns: new[] { "AnswerOptionId", "ParticipantAnswerId" },
                keyValues: new object[] { 194, 196 });

            migrationBuilder.DeleteData(
                table: "ParticipantSelectedOptions",
                keyColumns: new[] { "AnswerOptionId", "ParticipantAnswerId" },
                keyValues: new object[] { 195, 196 });

            migrationBuilder.DeleteData(
                table: "ParticipantSelectedOptions",
                keyColumns: new[] { "AnswerOptionId", "ParticipantAnswerId" },
                keyValues: new object[] { 194, 202 });

            migrationBuilder.DeleteData(
                table: "ParticipantSelectedOptions",
                keyColumns: new[] { "AnswerOptionId", "ParticipantAnswerId" },
                keyValues: new object[] { 196, 202 });

            migrationBuilder.DeleteData(
                table: "LiveQuizParticipants",
                keyColumn: "Id",
                keyValue: 1224);

            migrationBuilder.DeleteData(
                table: "LiveQuizParticipants",
                keyColumn: "Id",
                keyValue: 1225);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "ParticipantAnswers",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "LiveQuizParticipants",
                keyColumn: "Id",
                keyValue: 1222);

            migrationBuilder.DeleteData(
                table: "LiveQuizParticipants",
                keyColumn: "Id",
                keyValue: 1223);

            migrationBuilder.DeleteData(
                table: "GameRooms",
                keyColumn: "Id",
                keyValue: 1210);

            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 210,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { true, 85, "Tačno" });

            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 211,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { false, 85, "Netačno" });

            migrationBuilder.UpdateData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 212,
                columns: new[] { "IsCorrect", "QuestionID", "Text" },
                values: new object[] { false, 83, "Grčka" });
        }
    }
}
