namespace KvizHub.Api.Dtos.LiveResult
{
    public class LiveGlobalRankingDto
    {
        public int Position { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserProfilePictureUrl { get; set; }
        public double AverageScore { get; set; } 
        public int ArenasPlayed { get; set; }
    }
}
