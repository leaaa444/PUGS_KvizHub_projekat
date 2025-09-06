using KvizHub.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Models
{
	public class Quiz
	{
        [Key]
        public int QuizID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public QuizDifficulty Difficulty { get; set; }

        public int TimeLimit { get; set; }

        public bool IsArchived { get; set; } = false;

        public virtual List<Question> Questions { get; set; } = new();
    
        public virtual List<QuizCategory> QuizCategories { get; set; } = new();
    
        public virtual List<QuizResult> QuizResults { get; set; } = new();

    }
}
