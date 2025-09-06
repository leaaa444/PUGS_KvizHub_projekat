using KvizHub.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Mod
{
	public class UserAnswer
	{
        [Key]
        public int UserAnswerId { get; set; }

        [ForeignKey("QuizResult")]
        public int QuizResultID { get; set; }

        [ForeignKey("Question")]
        public int QuestionID { get; set; }

        [ForeignKey("SelectedAnswerOption")]
        public int? SelectedAnswerOptionID { get; set; }

        public string GivenTextAnswer { get; set; } = string.Empty;

        public virtual required QuizResult QuizResult { get; set; }

        public virtual required Question Question { get; set; }

        public virtual AnswerOption? SelectedAnswerOption { get; set; }
    }
}
