using KvizHub.Api.Models.Enums;

namespace KvizHub.Api.Dtos.Result
{
    public class ArchivedResultDetailsDto
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string Difficulty { get; set; } = string.Empty; 
        public List<string> Categories { get; set; } = [];
        public int TimeTaken { get; set; }
        public List<ArchivedQuestionDto> Questions { get; set; } = [];
    }

    public class ArchivedQuestionDto
    {
        public int QuestionId { get; set; }
        public QuestionType Type { get; set; }
        public string Text { get; set; } = string.Empty;
        public double Points { get; set; }
        public List<OptionResultDto> Options { get; set; } = []; 
        public UserAnswerResultDto UserAnswer { get; set; } = new(); 
    }
}