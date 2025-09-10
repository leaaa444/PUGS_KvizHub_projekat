using KvizHub.Api.Models.Enums;

namespace KvizHub.Api.Dtos.Result
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public bool IsCorrect { get; set; }
        public List<OptionResultDto> Options { get; set; } = [];
        public UserAnswerResultDto UserAnswer { get; set; } = new();
        public CorrectAnswerResultDto CorrectAnswer { get; set; } = new();
    }

    public class OptionResultDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class UserAnswerResultDto
    {
        public List<int> AnswerOptionIds { get; set; } = [];
        public string? AnswerText { get; set; }
    }

    public class CorrectAnswerResultDto
    {
        public List<int> AnswerOptionIds { get; set; } = [];
        public string? AnswerText { get; set; }
    }
}