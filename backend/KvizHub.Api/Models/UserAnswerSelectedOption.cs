using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
    public class UserAnswerSelectedOption
    {
        public int UserAnswerId { get; set; }

        [ForeignKey("UserAnswerId")]
        public virtual UserAnswer UserAnswer { get; set; } = null!;

        public int AnswerOptionId { get; set; }
        [ForeignKey("AnswerOptionId")]
        public virtual AnswerOption AnswerOption { get; set; } = null!;
    }
}