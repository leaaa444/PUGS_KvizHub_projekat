using KvizHub.Api.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Question
{
    public class UpdateQuestionDto
    {
        public int QuestionID { get; set; }
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        public double PointNum { get; set; }
        public QuestionType Type { get; set; }
        public string? CorrectTextAnswer { get; set; }
        public List<UpdateAnswerOptionDto> AnswerOptions { get; set; } = new();
    }
}