using KvizHub.Api.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Models
{
	public class User
	{
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual List<QuizResult> QuizResults { get; set; } = new();
    }
}
