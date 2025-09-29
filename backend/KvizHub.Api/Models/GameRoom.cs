using KvizHub.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Models
{
    public class GameRoom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoomCode { get; set; } = string.Empty;

        [Required]
        public int QuizId { get; set; }

        [Required]
        public string HostUsername { get; set; } = string.Empty;

        public GameStatus Status { get; set; } = GameStatus.Lobby;

        public int CurrentQuestionIndex { get; set; } = 0;

        public DateTime? CurrentQuestionStartTime { get; set; }

        public DateTime? HostDisconnectedAt { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;

        public virtual ICollection<LiveQuizParticipant> Participants { get; set; } = new List<LiveQuizParticipant>();

    }
}