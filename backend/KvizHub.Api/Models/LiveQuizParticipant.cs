using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KvizHub.Api.Models
{
    public class LiveQuizParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int Score { get; set; } = 0;

        public int GameRoomId { get; set; }

        public DateTime? DisconnectedAt { get; set; }

        [ForeignKey("GameRoomId")]
        public virtual GameRoom GameRoom { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<ParticipantAnswer> Answers { get; set; } = new List<ParticipantAnswer>();
    }
}
