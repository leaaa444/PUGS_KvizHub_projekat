using KvizHub.Api.Models;
using KvizHub.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace KvizHub.Api.Data
{
    public class KvizHubContext : DbContext
    {
        public KvizHubContext(DbContextOptions<KvizHubContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Konfiguracija za vezu između QuizCategory (rešava problem kompozitnog ključa)
            modelBuilder.Entity<QuizCategory>()
                .HasKey(qc => new { qc.QuizID, qc.CategoryID });

            modelBuilder.Entity<UserAnswerSelectedOption>()
                .HasKey(e => new { e.UserAnswerId, e.AnswerOptionId });

            modelBuilder.Entity<UserAnswer>()
                .HasMany(ua => ua.SelectedOptions) // UserAnswer ima mnogo SelectedOptions
                .WithOne(so => so.UserAnswer) // Svaki SelectedOption ima jedan UserAnswer
                .HasForeignKey(so => so.UserAnswerId) // Spoljni ključ je UserAnswerId
                .OnDelete(DeleteBehavior.Cascade); // Ako se obriše UserAnswer, obriši i unose u medjutabeli

            modelBuilder.Entity<AnswerOption>()
                .HasMany<UserAnswerSelectedOption>() // AnswerOption se nalazi u mnogo UserAnswerSelectedOption
                .WithOne(so => so.AnswerOption) // Svaki SelectedOption ima jedan AnswerOption
                .HasForeignKey(so => so.AnswerOptionId) // Spoljni ključ
                .OnDelete(DeleteBehavior.Restrict); // ako se obriše opcija, ne radi ništa automatski


            // Prekidanje lanca kaskadnog brisanja
            // Kažemo da veza od Question ka UserAnswer ne radi automatsko brisanje
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany() // Pošto Question nema listu UserAnswer-a, WithMany() je prazan
                .HasForeignKey(ua => ua.QuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParticipantSelectedOption>()
                .HasKey(pso => new { pso.ParticipantAnswerId, pso.AnswerOptionId });

            modelBuilder.Entity<ParticipantSelectedOption>()
                .HasOne(pso => pso.AnswerOption) // Svaki unos ima jednu opciju odgovora
                .WithMany() // AnswerOption nema direktnu listu ovih unosa
                .HasForeignKey(pso => pso.AnswerOptionId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ParticipantSelectedOption>()
                .HasOne(pso => pso.ParticipantAnswer)
                .WithMany(pa => pa.SelectedOptions)
                .HasForeignKey(pso => pso.ParticipantAnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameRoom>()
               .HasOne(gr => gr.Quiz)
               .WithMany() // Pošto Quiz nema direktnu listu GameRoom-ova
               .HasForeignKey(gr => gr.QuizId)
               .OnDelete(DeleteBehavior.Restrict);


            {// --- KVIZOVI ---
                modelBuilder.Entity<Quiz>().HasData(
                    new Quiz
                    {
                        QuizID = 16, // Počinjemo od 16
                        Name = "Brzi Izazov Znanja",
                        Description = "Lagani kviz opšte kulture za zagrevanje.",
                        Difficulty = QuizDifficulty.Easy,
                        TimeLimit = 0,
                        Mode = QuizMode.Live,
                        IsArchived = false
                    },
                    new Quiz
                    {
                        QuizID = 17,
                        Name = "Put oko sveta",
                        Description = "Proveri svoje znanje iz geografije u ovom kvizu srednje težine.",
                        Difficulty = QuizDifficulty.Medium,
                        TimeLimit = 0,
                        Mode = QuizMode.Live,
                        IsArchived = false
                    },
                    new Quiz
                    {
                        QuizID = 18,
                        Name = "Developerski Duel",
                        Description = "Težak kviz za sve programere. Dokaži svoje znanje!",
                        Difficulty = QuizDifficulty.Hard,
                        TimeLimit = 0,
                        Mode = QuizMode.Live,
                        IsArchived = false
                    }
                );

                // --- PITANJA ---
                modelBuilder.Entity<Question>().HasData(
                    // Pitanja za Kviz 1 (ID 16)
                    new Question { QuestionID = 73, QuizID = 16, PointNum = 100, Type = QuestionType.TrueFalse, QuestionText = "Da li je Pariz glavni grad Francuske?", TimeLimitSeconds = 10 },
                    new Question { QuestionID = 74, QuizID = 16, PointNum = 150, Type = QuestionType.SingleChoice, QuestionText = "Koja planeta je poznata kao Crvena planeta?", TimeLimitSeconds = 15 },
                    new Question { QuestionID = 75, QuizID = 16, PointNum = 200, Type = QuestionType.MultipleChoice, QuestionText = "Koje od navedenih životinja su sisari?", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 76, QuizID = 16, PointNum = 100, Type = QuestionType.FillInTheBlank, QuestionText = "Glavni grad Japana je ____.", CorrectTextAnswer = "Tokio", TimeLimitSeconds = 15 },
                    new Question { QuestionID = 77, QuizID = 16, PointNum = 150, Type = QuestionType.SingleChoice, QuestionText = "Koliko srca ima oktopod?", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 78, QuizID = 16, PointNum = 100, Type = QuestionType.TrueFalse, QuestionText = "Da li Sunce izlazi na zapadu?", TimeLimitSeconds = 10 },

                    // Pitanja za Kviz 2 (ID 17)
                    new Question { QuestionID = 79, QuizID = 17, PointNum = 200, Type = QuestionType.SingleChoice, QuestionText = "Koja je najduža reka na svetu?", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 80, QuizID = 17, PointNum = 150, Type = QuestionType.FillInTheBlank, QuestionText = "Najviši planinski vrh na svetu je Mont ____.", CorrectTextAnswer = "Everest", TimeLimitSeconds = 15 },
                    new Question { QuestionID = 81, QuizID = 17, PointNum = 150, Type = QuestionType.TrueFalse, QuestionText = "Da li se Australija nalazi na južnoj hemisferi?", TimeLimitSeconds = 10 },
                    new Question { QuestionID = 82, QuizID = 17, PointNum = 250, Type = QuestionType.MultipleChoice, QuestionText = "Koje od navedenih država se nalaze u Južnoj Americi?", TimeLimitSeconds = 25 },
                    new Question { QuestionID = 83, QuizID = 17, PointNum = 200, Type = QuestionType.SingleChoice, QuestionText = "U kojoj državi se nalazi drevni grad Petra?", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 84, QuizID = 17, PointNum = 150, Type = QuestionType.FillInTheBlank, QuestionText = "Pustinja Sahara se nalazi na kontinentu ____.", CorrectTextAnswer = "Afrika", TimeLimitSeconds = 15 },

                    // Pitanja za Kviz 3 (ID 18)
                    new Question { QuestionID = 85, QuizID = 18, PointNum = 200, Type = QuestionType.TrueFalse, QuestionText = "Da li je C# statički tipiziran jezik?", TimeLimitSeconds = 15 },
                    new Question { QuestionID = 86, QuizID = 18, PointNum = 250, Type = QuestionType.FillInTheBlank, QuestionText = "U React-u, hook za upravljanje stanjem u funkcijskim komponentama je ____State.", CorrectTextAnswer = "useState", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 87, QuizID = 18, PointNum = 300, Type = QuestionType.SingleChoice, QuestionText = "Šta znači 'DRY' princip u programiranju?", TimeLimitSeconds = 20 },
                    new Question { QuestionID = 88, QuizID = 18, PointNum = 350, Type = QuestionType.MultipleChoice, QuestionText = "Koje od navedenih su validne HTTP metode?", TimeLimitSeconds = 25 },
                    new Question { QuestionID = 89, QuizID = 18, PointNum = 300, Type = QuestionType.SingleChoice, QuestionText = "Koji upit će EF Core prevesti u SQL bez praćenja promena?", TimeLimitSeconds = 25 },
                    new Question { QuestionID = 90, QuizID = 18, PointNum = 250, Type = QuestionType.FillInTheBlank, QuestionText = "SQL komanda za brisanje cele tabele iz baze je DROP ____.", CorrectTextAnswer = "TABLE", TimeLimitSeconds = 20 }
                );

                // --- PONUĐENI ODGOVORI ---
                modelBuilder.Entity<AnswerOption>().HasData(
                    // Odgovori za Kviz 1 (pitanja 73-78)
                    new AnswerOption { AnswerOptionID = 189, QuestionID = 73, Text = "Tačno", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 190, QuestionID = 73, Text = "Netačno", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 191, QuestionID = 74, Text = "Mars", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 192, QuestionID = 74, Text = "Venera", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 193, QuestionID = 74, Text = "Jupiter", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 194, QuestionID = 75, Text = "Kit", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 195, QuestionID = 75, Text = "Slepi miš", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 196, QuestionID = 75, Text = "Ajkula", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 197, QuestionID = 77, Text = "Jedno", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 198, QuestionID = 77, Text = "Dva", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 199, QuestionID = 77, Text = "Tri", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 200, QuestionID = 78, Text = "Tačno", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 201, QuestionID = 78, Text = "Netačno", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 202, QuestionID = 79, Text = "Amazon", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 203, QuestionID = 79, Text = "Nil", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 204, QuestionID = 79, Text = "Grčka", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 205, QuestionID = 81, Text = "Tačno", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 206, QuestionID = 81, Text = "Netačno", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 207, QuestionID = 82, Text = "Brazil", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 208, QuestionID = 82, Text = "Meksiko", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 209, QuestionID = 82, Text = "Argentina", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 210, QuestionID = 83, Text = "Egipat", IsCorrect = false },
                    new AnswerOption { AnswerOptionID = 211, QuestionID = 83, Text = "Jordan", IsCorrect = true },

                    // Odgovori za Kviz 3 (pitanja 85-90)
                    new AnswerOption { AnswerOptionID = 212, QuestionID = 85, Text = "Tačno", IsCorrect = true },
                    new AnswerOption { AnswerOptionID = 213, QuestionID = 85, Text = "Netačno", IsCorrect = false },
                new AnswerOption { AnswerOptionID = 215, QuestionID = 87, Text = "Do Not Repeat Yourself", IsCorrect = false },
                new AnswerOption { AnswerOptionID = 216, QuestionID = 87, Text = "Don't Repeat Yourself", IsCorrect = true },
                new AnswerOption { AnswerOptionID = 217, QuestionID = 87, Text = "Drive Route Yellow", IsCorrect = false },
                new AnswerOption { AnswerOptionID = 218, QuestionID = 88, Text = "GET", IsCorrect = true },
                new AnswerOption { AnswerOptionID = 219, QuestionID = 88, Text = "POST", IsCorrect = true },
                new AnswerOption { AnswerOptionID = 220, QuestionID = 88, Text = "FETCH", IsCorrect = false },
                new AnswerOption { AnswerOptionID = 221, QuestionID = 88, Text = "PUT", IsCorrect = true },
                new AnswerOption { AnswerOptionID = 222, QuestionID = 89, Text = "FirstOrDefault()", IsCorrect = false },
                new AnswerOption { AnswerOptionID = 223, QuestionID = 89, Text = "AsNoTracking()", IsCorrect = true },
                new AnswerOption { AnswerOptionID = 224, QuestionID = 89, Text = "Include()", IsCorrect = false }
                );

                // --- VEZA IZMEĐU KVIZA I KATEGORIJE ---
                modelBuilder.Entity<QuizCategory>().HasData(
                    new QuizCategory { QuizID = 16, CategoryID = 13 },
                    new QuizCategory { QuizID = 17, CategoryID = 8 },
                    new QuizCategory { QuizID = 18, CategoryID = 1 }
                );


                // ---ZAVRŠENA PARTIJA-- -
                modelBuilder.Entity<GameRoom>().HasData(
        new GameRoom
        {
            Id = 1210, // Tvoj poslednji ID
            RoomCode = "SEED01",
            QuizId = 16, // Povezuje se sa kvizom ID 16 koji je definisan gore
            HostUsername = "djordje",
            Status = GameStatus.Finished,
            FinishedAt = new DateTime(2025, 9, 29, 14, 30, 0, DateTimeKind.Utc)
        }
    );

                // --- UČESNICI TE PARTIJE ---
                modelBuilder.Entity<LiveQuizParticipant>().HasData(
                    new LiveQuizParticipant { Id = 1222, UserId = 2, Score = 960, GameRoomId = 1210 },
                    new LiveQuizParticipant { Id = 1223, UserId = 6, Score = 610, GameRoomId = 1210 },
                    new LiveQuizParticipant { Id = 1224, UserId = 7, Score = 300, GameRoomId = 1210 },
                    new LiveQuizParticipant { Id = 1225, UserId = 8, Score = 110, GameRoomId = 1210 }
                );

                // --- ODGOVORI UČESNIKA ---
                modelBuilder.Entity<ParticipantAnswer>().HasData(
                    // --- Odgovori za maca_lukas (ParticipantId = 1222) ---
    new ParticipantAnswer { Id = 194, ParticipantId = 1222, QuestionId = 71, IsCorrect = true, AnswerTimeMilliseconds = 2500, PointsAwarded = 175, SubmittedAnswer = "Tačno" },
    new ParticipantAnswer { Id = 195, ParticipantId = 1222, QuestionId = 72, IsCorrect = true, AnswerTimeMilliseconds = 4000, PointsAwarded = 220, SubmittedAnswer = "Mars" },
    new ParticipantAnswer { Id = 196, ParticipantId = 1222, QuestionId = 73, IsCorrect = true, AnswerTimeMilliseconds = 6000, PointsAwarded = 280, SubmittedAnswer = "-6,-7" },
    new ParticipantAnswer { Id = 197, ParticipantId = 1222, QuestionId = 74, IsCorrect = false, AnswerTimeMilliseconds = 5000, PointsAwarded = 0, SubmittedAnswer = "Osaka" },
    new ParticipantAnswer { Id = 198, ParticipantId = 1222, QuestionId = 75, IsCorrect = true, AnswerTimeMilliseconds = 7000, PointsAwarded = 210, SubmittedAnswer = "-11" },
    new ParticipantAnswer { Id = 199, ParticipantId = 1222, QuestionId = 76, IsCorrect = true, AnswerTimeMilliseconds = 1500, PointsAwarded = 75, SubmittedAnswer = "Netačno" },

    // --- Odgovori za user2 (ParticipantId = 1223) ---
    new ParticipantAnswer { Id = 200, ParticipantId = 1223, QuestionId = 71, IsCorrect = true, AnswerTimeMilliseconds = 4000, PointsAwarded = 160, SubmittedAnswer = "Tačno" },
    new ParticipantAnswer { Id = 201, ParticipantId = 1223, QuestionId = 72, IsCorrect = true, AnswerTimeMilliseconds = 8000, PointsAwarded = 192, SubmittedAnswer = "Mars" },
    new ParticipantAnswer { Id = 202, ParticipantId = 1223, QuestionId = 73, IsCorrect = false, AnswerTimeMilliseconds = 9000, PointsAwarded = 0, SubmittedAnswer = "-6,-8" },
    new ParticipantAnswer { Id = 203, ParticipantId = 1223, QuestionId = 74, IsCorrect = true, AnswerTimeMilliseconds = 6000, PointsAwarded = 140, SubmittedAnswer = "Tokio" },
    new ParticipantAnswer { Id = 204, ParticipantId = 1223, QuestionId = 75, IsCorrect = true, AnswerTimeMilliseconds = 11000, PointsAwarded = 172, SubmittedAnswer = "-11" },
    new ParticipantAnswer { Id = 205, ParticipantId = 1223, QuestionId = 76, IsCorrect = false, AnswerTimeMilliseconds = 3000, PointsAwarded = 0, SubmittedAnswer = "Tačno" },

    // --- Odgovori za user3 (ParticipantId = 1224) ---
    new ParticipantAnswer { Id = 206, ParticipantId = 1224, QuestionId = 71, IsCorrect = false, AnswerTimeMilliseconds = 5000, PointsAwarded = 0, SubmittedAnswer = "Netačno" },
    new ParticipantAnswer { Id = 207, ParticipantId = 1224, QuestionId = 72, IsCorrect = true, AnswerTimeMilliseconds = 12000, PointsAwarded = 165, SubmittedAnswer = "Mars" },
    new ParticipantAnswer { Id = 208, ParticipantId = 1224, QuestionId = 74, IsCorrect = true, AnswerTimeMilliseconds = 10000, PointsAwarded = 116, SubmittedAnswer = "Tokio" },
    new ParticipantAnswer { Id = 209, ParticipantId = 1224, QuestionId = 76, IsCorrect = false, AnswerTimeMilliseconds = 6000, PointsAwarded = 0, SubmittedAnswer = "Tačno" },

    // --- Odgovori za user4 (ParticipantId = 1225) ---
    new ParticipantAnswer { Id = 210, ParticipantId = 1225, QuestionId = 71, IsCorrect = true, AnswerTimeMilliseconds = 9000, PointsAwarded = 110, SubmittedAnswer = "Tačno" },
    new ParticipantAnswer { Id = 211, ParticipantId = 1225, QuestionId = 72, IsCorrect = false, AnswerTimeMilliseconds = 14000, PointsAwarded = 0, SubmittedAnswer = "Jupiter" }

                );

                // --- SELEKTOVANE OPCIJE ---
                modelBuilder.Entity<ParticipantSelectedOption>().HasData(
                    new { ParticipantAnswerId = 196, AnswerOptionId = 194 },
                    new { ParticipantAnswerId = 196, AnswerOptionId = 195 },
                    new { ParticipantAnswerId = 202, AnswerOptionId = 194 },
                    new { ParticipantAnswerId = 202, AnswerOptionId = 196 }
                );

                // --- NOVA ZAVRŠENA PARTIJA #2 (Kviz: Put oko sveta, ID 17) ---
                modelBuilder.Entity<GameRoom>().HasData(
                    new GameRoom
                    {
                        Id = 1211, // Novi ID
                        RoomCode = "SEED02",
                        QuizId = 17, // Povezuje se sa kvizom "Put oko sveta"
                        HostUsername = "maca_lukas",
                        Status = GameStatus.Finished,
                        FinishedAt = new DateTime(2025, 9, 28, 18, 0, 0, DateTimeKind.Utc)
                    }
                );

                // --- UČESNICI PARTIJE #2 ---
                modelBuilder.Entity<LiveQuizParticipant>().HasData(
                    new LiveQuizParticipant { Id = 1226, UserId = 1, Score = 850, GameRoomId = 1211 },
                    new LiveQuizParticipant { Id = 1227, UserId = 6, Score = 550, GameRoomId = 1211 },
                    new LiveQuizParticipant { Id = 1228, UserId = 8, Score = 400, GameRoomId = 1211 }
                );

                // --- ODGOVORI UČESNIKA PARTIJE #2 ---
                modelBuilder.Entity<ParticipantAnswer>().HasData(
                    // Odgovori za djordje (ParticipantId = 1226)
                    new ParticipantAnswer { Id = 212, ParticipantId = 1226, QuestionId = 79, IsCorrect = true, AnswerTimeMilliseconds = 5000, PointsAwarded = 250, SubmittedAnswer = "203" },
                    new ParticipantAnswer { Id = 213, ParticipantId = 1226, QuestionId = 80, IsCorrect = true, AnswerTimeMilliseconds = 3000, PointsAwarded = 200, SubmittedAnswer = "Everest" },
                    new ParticipantAnswer { Id = 214, ParticipantId = 1226, QuestionId = 81, IsCorrect = true, AnswerTimeMilliseconds = 2000, PointsAwarded = 150, SubmittedAnswer = "205" },
                    new ParticipantAnswer { Id = 215, ParticipantId = 1226, QuestionId = 82, IsCorrect = false, AnswerTimeMilliseconds = 10000, PointsAwarded = 0, SubmittedAnswer = "207,208" }, // Pogrešan (Meksiko)
                    new ParticipantAnswer { Id = 216, ParticipantId = 1226, QuestionId = 83, IsCorrect = true, AnswerTimeMilliseconds = 6000, PointsAwarded = 250, SubmittedAnswer = "211" },

                    // Odgovori za user2 (ParticipantId = 1227)
                    new ParticipantAnswer { Id = 217, ParticipantId = 1227, QuestionId = 79, IsCorrect = false, AnswerTimeMilliseconds = 7000, PointsAwarded = 0, SubmittedAnswer = "202" },
                    new ParticipantAnswer { Id = 218, ParticipantId = 1227, QuestionId = 81, IsCorrect = true, AnswerTimeMilliseconds = 4000, PointsAwarded = 150, SubmittedAnswer = "205" },
                    new ParticipantAnswer { Id = 219, ParticipantId = 1227, QuestionId = 82, IsCorrect = true, AnswerTimeMilliseconds = 12000, PointsAwarded = 400, SubmittedAnswer = "207,209" } // Tačan
                );

                // --- SELEKTOVANE OPCIJE PARTIJE #2 ---
                modelBuilder.Entity<ParticipantSelectedOption>().HasData(
                    new { ParticipantAnswerId = 215, AnswerOptionId = 207 },
                    new { ParticipantAnswerId = 215, AnswerOptionId = 208 },
                    new { ParticipantAnswerId = 219, AnswerOptionId = 207 },
                    new { ParticipantAnswerId = 219, AnswerOptionId = 209 }
                );

                // --- NOVA ZAVRŠENA PARTIJA #3 (Kviz: Developerski Duel, ID 18) ---
                modelBuilder.Entity<GameRoom>().HasData(
                    new GameRoom
                    {
                        Id = 1212, // Novi ID
                        RoomCode = "SEED03",
                        QuizId = 18, // Povezuje se sa kvizom "Developerski Duel"
                        HostUsername = "user2",
                        Status = GameStatus.Finished,
                        FinishedAt = new DateTime(2025, 9, 27, 20, 0, 0, DateTimeKind.Utc)
                    }
                );

                // --- UČESNICI PARTIJE #3 ---
                modelBuilder.Entity<LiveQuizParticipant>().HasData(
                    new LiveQuizParticipant { Id = 1229, UserId = 1, Score = 1150, GameRoomId = 1212 },
                    new LiveQuizParticipant { Id = 1230, UserId = 2, Score = 850, GameRoomId = 1212 }
                );

                // --- ODGOVORI UČESNIKA PARTIJE #3 ---
                modelBuilder.Entity<ParticipantAnswer>().HasData(
                    // Odgovori za djordje (ParticipantId = 1229)
                    new ParticipantAnswer { Id = 220, ParticipantId = 1229, QuestionId = 85, IsCorrect = true, AnswerTimeMilliseconds = 3000, PointsAwarded = 250, SubmittedAnswer = "212" },
                    new ParticipantAnswer { Id = 221, ParticipantId = 1229, QuestionId = 86, IsCorrect = true, AnswerTimeMilliseconds = 5000, PointsAwarded = 300, SubmittedAnswer = "useState" },
                    new ParticipantAnswer { Id = 222, ParticipantId = 1229, QuestionId = 87, IsCorrect = true, AnswerTimeMilliseconds = 4000, PointsAwarded = 350, SubmittedAnswer = "216" },
                    new ParticipantAnswer { Id = 223, ParticipantId = 1229, QuestionId = 89, IsCorrect = true, AnswerTimeMilliseconds = 6000, PointsAwarded = 250, SubmittedAnswer = "223" },

                    // Odgovori za maca_lukas (ParticipantId = 1230)
                    new ParticipantAnswer { Id = 224, ParticipantId = 1230, QuestionId = 85, IsCorrect = true, AnswerTimeMilliseconds = 4000, PointsAwarded = 200, SubmittedAnswer = "212" },
                    new ParticipantAnswer { Id = 225, ParticipantId = 1230, QuestionId = 86, IsCorrect = true, AnswerTimeMilliseconds = 6000, PointsAwarded = 250, SubmittedAnswer = "useState" },
                    new ParticipantAnswer { Id = 226, ParticipantId = 1230, QuestionId = 88, IsCorrect = true, AnswerTimeMilliseconds = 8000, PointsAwarded = 400, SubmittedAnswer = "218,219,221" }
                );

                // --- SELEKTOVANE OPCIJE PARTIJE #3 ---
                modelBuilder.Entity<ParticipantSelectedOption>().HasData(
                    new { ParticipantAnswerId = 226, AnswerOptionId = 218 },
                    new { ParticipantAnswerId = 226, AnswerOptionId = 219 },
                    new { ParticipantAnswerId = 226, AnswerOptionId = 221 }
                );
            }


        }


        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserAnswerSelectedOption> UserAnswerSelectedOptions { get; set; }
        public DbSet<GameRoom> GameRooms { get; set; }
        public DbSet<LiveQuizParticipant> LiveQuizParticipants { get; set; }
        public DbSet<ParticipantAnswer> ParticipantAnswers { get; set; }
        public DbSet<ParticipantSelectedOption> ParticipantSelectedOptions { get; set; }

    }
}