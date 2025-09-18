using System.ComponentModel.DataAnnotations;
namespace KvizHub.Api.Dtos.Account
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna.")]
        public string Email { get; set; } = string.Empty;
    }
}