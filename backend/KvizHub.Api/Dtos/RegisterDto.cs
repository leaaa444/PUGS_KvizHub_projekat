﻿using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; }  = string.Empty;

        // Sliku za sada nećemo slati pri registraciji,
        // korisnik je može dodati kasnije.
    }
}