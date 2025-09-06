using KvizHub.Api.Mod;
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
    }
}