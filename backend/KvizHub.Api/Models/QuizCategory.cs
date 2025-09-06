using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
	public class QuizCategory
	{
        [Key]
		public int QuizCategoryID { get; set; }

        [ForeignKey("Quiz")]
        public int QuizID { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public virtual required Quiz Quiz { get; set; }

        public virtual required Category Category { get; set; }
    }
}
