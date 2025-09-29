using KvizHub.Api.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Question
{
    public class CreateQuestionDto
    {
        public int QuestionID { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;
        public double PointNum { get; set; } = 1;
        public QuestionType Type { get; set; }
        public string? CorrectTextAnswer { get; set; }
        public int? TimeLimitSeconds { get; set; }

        public List<CreateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}