namespace KvizHub.Api.Dtos.Result
{
    public class GlobalRankingDto
    {
        public int Position { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserProfilePictureUrl { get; set; } = string.Empty;
        public double GlobalScore { get; set; }
        public int QuizzesPlayed { get; set; } 
    }
}