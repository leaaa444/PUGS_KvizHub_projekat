namespace KvizHub.Api.Dtos.Result
{
    public class MyResultDto
    {
        public int ResultId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public DateTime DateCompleted { get; set; }
        public double Score { get; set; }
        public double Percentage { get; set; }
        public int AttemptNum { get; set; }
    }
}