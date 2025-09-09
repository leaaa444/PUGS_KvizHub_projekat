using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Question
{
    public class QuestionDto
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double PointNum { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? CorrectTextAnswer { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}