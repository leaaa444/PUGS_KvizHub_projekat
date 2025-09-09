using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
	public class QuizResult
	{
        [Key]
        public int QuizResultID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Quiz")]
        public int QuizID { get; set; }

        public int CompletionTime { get; set; }

        public double Score { get; set; }

        public int CorrectAnswers { get; set; }

        public DateTime DateOfCompletion { get; set; }

        public int AttemptNum { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual List<UserAnswer> UserAnswers { get; set; } = new();

        public virtual Quiz Quiz { get; set; } = null!;

    }
}
