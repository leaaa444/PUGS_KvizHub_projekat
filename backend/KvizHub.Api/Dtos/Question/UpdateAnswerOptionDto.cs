using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Question
{
    public class UpdateAnswerOptionDto
    {
        public int AnswerOptionID { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}