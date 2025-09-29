namespace KvizHub.Api.Dtos.Player
{
    public class PlayerDto
    {
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDisconnected { get; set; }
    }
}
