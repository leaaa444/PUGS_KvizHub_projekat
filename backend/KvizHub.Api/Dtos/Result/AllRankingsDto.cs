namespace KvizHub.Api.Dtos.Result
{
    public class AllRankingsDto
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public List<RankingEntryDto> TopPlayers { get; set; } = [];
    }

    public class RankingEntryDto
    {
        public string Username { get; set; } = string.Empty;
        public string UserProfilePictureUrl { get; set; } = string.Empty;
        public double Score { get; set; }
        public int TimeTaken { get; set; }
        public DateTime DateCompleted { get; set; }
    }
}