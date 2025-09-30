namespace KvizHub.Api.Dtos.LiveResult
{
    public class MyFinishedArenaDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public DateTime FinishedAt { get; set; }
        public int YourScore { get; set; }
        public int YourRank { get; set; }
        public int ParticipantCount { get; set; }
    }
}
