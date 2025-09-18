using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Account
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Nova lozinka mora imati najmanje 6 karaktera.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}