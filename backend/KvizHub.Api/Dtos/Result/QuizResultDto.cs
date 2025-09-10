namespace KvizHub.Api.Dtos.Result
{
    public class QuizResultDto
    {
        public int ResultId { get; set; }
        public int QuizId { get; set; }
        public double Score { get; set; }
        public double MaxPossibleScore { get; set; }
        public int CorrectAnswers { get; set; }
    }
}