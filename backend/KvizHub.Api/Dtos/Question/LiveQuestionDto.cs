namespace KvizHub.Api.Dtos.Question
{
    public class LiveQuestionDto : QuestionDto
    {
        public int CurrentQuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
    }
}
