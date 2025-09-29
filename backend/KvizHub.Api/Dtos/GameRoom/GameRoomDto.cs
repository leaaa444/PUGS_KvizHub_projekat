using KvizHub.Api.Dtos.Player;

namespace KvizHub.Api.Dtos.GameRoom
{
    public class GameRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public string HostUsername { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int CurrentQuestionIndex { get; set; }
        public DateTime? CurrentQuestionStartTime { get; set; }
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
    }
}
