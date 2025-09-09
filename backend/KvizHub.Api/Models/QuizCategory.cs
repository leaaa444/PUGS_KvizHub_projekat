using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
	public class QuizCategory
	{
        [Key]
        public int QuizID { get; set; }

        [Key]
        public int CategoryID { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;

        public virtual Category Category { get; set; } = null!;
    }
}
