using KvizHub.Api.Models;
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

    }
}