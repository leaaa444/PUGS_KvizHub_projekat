namespace KvizHub.Api.Dtos.LiveResult
{
    public class FinishedArenaDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public string HostUsername { get; set; } = string.Empty;
        public DateTime FinishedAt { get; set; }
        public int ParticipantCount { get; set; }
        public string WinnerUsername { get; set; } = string.Empty;
        public int WinnerScore { get; set; }
    }
}
