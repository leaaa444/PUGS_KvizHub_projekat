using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KvizHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedLiveQuizzesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "QuizID", "Description", "Difficulty", "IsArchived", "Mode", "Name", "TimeLimit" },
                values: new object[,]
                {
                    { 16, "Lagani kviz opšte kulture za zagrevanje.", 0, false, 1, "Brzi Izazov Znanja", 0 },
                    { 17, "Proveri svoje znanje iz geografije u ovom kvizu srednje težine.", 1, false, 1, "Put oko sveta", 0 },
                    { 18, "Težak kviz za sve programere. Dokaži svoje znanje!", 2, false, 1, "Developerski Duel", 0 }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionID", "CorrectTextAnswer", "IsArchived", "PointNum", "QuestionText", "QuizID", "TimeLimitSeconds", "Type" },
                values: new object[,]
                {
                    { 73, "", false, 100.0, "Da li je Pariz glavni grad Francuske?", 16, 10, 2 },
                    { 74, "", false, 150.0, "Koja planeta je poznata kao Crvena planeta?", 16, 15, 0 },
                    { 75, "", false, 200.0, "Koje od navedenih životinja su sisari?", 16, 20, 1 },
                    { 76, "Tokio", false, 100.0, "Glavni grad Japana je ____.", 16, 15, 3 },
                    { 77, "", false, 150.0, "Koliko srca ima oktopod?", 16, 20, 0 },
                    { 78, "", false, 100.0, "Da li Sunce izlazi na zapadu?", 16, 10, 2 },
                    { 79, "", false, 200.0, "Koja je najduža reka na svetu?", 17, 20, 0 },
                    { 80, "Everest", false, 150.0, "Najviši planinski vrh na svetu je Mont ____.", 17, 15, 3 },
                    { 81, "", false, 150.0, "Da li se Australija nalazi na južnoj hemisferi?", 17, 10, 2 },
                    { 82, "", false, 250.0, "Koje od navedenih država se nalaze u Južnoj Americi?", 17, 25, 1 },
                    { 83, "", false, 200.0, "U kojoj državi se nalazi drevni grad Petra?", 17, 20, 0 },
                    { 84, "Afrika", false, 150.0, "Pustinja Sahara se nalazi na kontinentu ____.", 17, 15, 3 },
                    { 85, "", false, 200.0, "Da li je C# statički tipiziran jezik?", 18, 15, 2 },
                    { 86, "useState", false, 250.0, "U React-u, hook za upravljanje stanjem u funkcijskim komponentama je ____State.", 18, 20, 3 },
                    { 87, "", false, 300.0, "Šta znači 'DRY' princip u programiranju?", 18, 20, 0 },
                    { 88, "", false, 350.0, "Koje od navedenih su validne HTTP metode?", 18, 25, 1 },
                    { 89, "", false, 300.0, "Koji upit će EF Core prevesti u SQL bez praćenja promena?", 18, 25, 0 },
                    { 90, "TABLE", false, 250.0, "SQL komanda za brisanje cele tabele iz baze je DROP ____.", 18, 20, 3 }
                });

            migrationBuilder.InsertData(
                table: "QuizCategories",
                columns: new[] { "CategoryID", "QuizID" },
                values: new object[,]
                {
                    { 13, 16 },
                    { 8, 17 },
                    { 1, 18 }
                });

            migrationBuilder.InsertData(
                table: "AnswerOptions",
                columns: new[] { "AnswerOptionID", "IsCorrect", "QuestionID", "Text" },
                values: new object[,]
                {
                    { 189, true, 73, "Tačno" },
                    { 190, false, 73, "Netačno" },
                    { 191, true, 74, "Mars" },
                    { 192, false, 74, "Venera" },
                    { 193, false, 74, "Jupiter" },
                    { 194, true, 75, "Kit" },
                    { 195, true, 75, "Slepi miš" },
                    { 196, false, 75, "Ajkula" },
                    { 197, false, 77, "Jedno" },
                    { 198, false, 77, "Dva" },
                    { 199, true, 77, "Tri" },
                    { 200, false, 78, "Tačno" },
                    { 201, true, 78, "Netačno" },
                    { 202, false, 79, "Amazon" },
                    { 203, true, 79, "Nil" },
                    { 210, true, 85, "Tačno" },
                    { 211, false, 85, "Netačno" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "AnswerOptions",
                keyColumn: "AnswerOptionID",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "QuizCategories",
                keyColumns: new[] { "CategoryID", "QuizID" },
                keyValues: new object[] { 13, 16 });

            migrationBuilder.DeleteData(
                table: "QuizCategories",
                keyColumns: new[] { "CategoryID", "QuizID" },
                keyValues: new object[] { 8, 17 });

            migrationBuilder.DeleteData(
                table: "QuizCategories",
                keyColumns: new[] { "CategoryID", "QuizID" },
                keyValues: new object[] { 1, 18 });

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "QuestionID",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "QuizID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "QuizID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Quizzes",
                keyColumn: "QuizID",
                keyValue: 18);
        }
    }
}
