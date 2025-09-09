namespace KvizHub.Api.Dtos.Quiz
{
    public class QuizListDto
    {
        public int QuizID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int TimesCompleted { get; set; }
        public double MaxPoints { get; set; }
        public int NumberOfQuestions { get; set; }
        public List<string> Categories { get; set; } = new();
    }
}