using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
    public class ParticipantAnswer
    {
        [Key]
        public int Id { get; set; }

        public int ParticipantId { get; set; }

        public int QuestionId { get; set; }

        public string SubmittedAnswer { get; set; } = string.Empty;

        public int AnswerTimeMilliseconds { get; set; } 

        public bool IsCorrect { get; set; }

        public int PointsAwarded { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual LiveQuizParticipant Participant { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; } = null!;

        public virtual ICollection<ParticipantSelectedOption> SelectedOptions { get; set; } = new List<ParticipantSelectedOption>();

    }
}