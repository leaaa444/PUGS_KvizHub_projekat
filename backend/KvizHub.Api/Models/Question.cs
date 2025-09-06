using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KvizHub.Api.Models.Enums;
using KvizHub.Api.Mod;

namespace KvizHub.Api.Models
{
	public class Question
	{
		[Key]
		public int QuestionID { get; set; }

		[ForeignKey("Quiz")]
		public int QuizID { get; set; }

		public double PointNum { get; set; }

		public QuestionType Type { get; set; }

		public string QuestionText { get; set; } = string.Empty;

		public string CorrectTextAnswer { get; set; } = string.Empty;

        public bool IsArchived { get; set; } = false;

        public virtual List<AnswerOption> AnswerOptions { get; set; } = new();

		public virtual required Quiz Quiz { get; set; }

	}
}
