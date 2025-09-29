using KvizHub.Api.Dtos.AnswerOption;
using KvizHub.Api.Models.Enums;
using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Question
{
    public class QuestionForQuizTakerDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public double PointNum { get; set; }
        public int? TimeLimitSeconds { get; set; }
        public List<AnswerOptionForQuizTakerDto> AnswerOptions { get; set; } = new();
    }
}