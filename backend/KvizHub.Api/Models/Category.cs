using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Models
{
	public class Category
	{
		[Key]
		public int CategoryID { get; set; }

		public string Name { get; set; } = string.Empty;

        public bool IsArchived { get; set; } = false;

        public virtual List<QuizCategory> QuizCategories { get; set; } = new();

	}
}
