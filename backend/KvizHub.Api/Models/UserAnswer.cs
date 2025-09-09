using KvizHub.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
	public class UserAnswer
	{
        [Key]
        public int UserAnswerId { get; set; }

        [ForeignKey("QuizResult")]
        public int QuizResultID { get; set; }

        [ForeignKey("Question")]
        public int QuestionID { get; set; }

        public string GivenTextAnswer { get; set; } = string.Empty;

        public virtual QuizResult QuizResult { get; set; } = null!;

        public virtual Question Question { get; set; } = null!;

        public virtual List<UserAnswerSelectedOption> SelectedOptions { get; set; } = new();


    }
}
