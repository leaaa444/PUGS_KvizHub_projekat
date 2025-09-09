namespace KvizHub.Api.Dtos.Question
{
    public class AnswerOptionDto
    {
        public int AnswerOptionID { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}