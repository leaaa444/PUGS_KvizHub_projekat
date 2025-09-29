using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
    public class ParticipantSelectedOption
    {
        public int ParticipantAnswerId { get; set; }
        [ForeignKey("ParticipantAnswerId")]
        public virtual ParticipantAnswer ParticipantAnswer { get; set; } = null!;

        public int AnswerOptionId { get; set; }
        [ForeignKey("AnswerOptionId")]
        public virtual AnswerOption AnswerOption { get; set; } = null!;
    }
}