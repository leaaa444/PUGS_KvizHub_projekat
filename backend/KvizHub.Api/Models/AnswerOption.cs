using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
	public class AnswerOption
	{
		[Key]
		public int AnswerOptionID { get; set; }

		[ForeignKey("Question")]
		public int QuestionID { get; set; }

		public string Text { get; set; } = string.Empty;

		public bool IsCorrect { get; set; }

		public virtual Question Question { get; set; } = null!;

    }
}
