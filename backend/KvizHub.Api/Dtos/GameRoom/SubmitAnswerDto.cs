namespace KvizHub.Api.Dtos.GameRoom
{
    public class SubmitAnswerDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new List<int>();
        public string? TextAnswer { get; set; }
    }
}
