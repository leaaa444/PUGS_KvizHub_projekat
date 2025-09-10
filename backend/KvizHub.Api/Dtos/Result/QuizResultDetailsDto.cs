namespace KvizHub.Api.Dtos.Result
{
    public class QuizResultDetailsDto
    {
        public int ResultId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public double Score { get; set; }
        public double MaxPossibleScore { get; set; }
        public DateTime DateCompleted { get; set; }
        public int TimeTaken { get; set; }
        public List<QuestionResultDto> Questions { get; set; } = [];
    }
}