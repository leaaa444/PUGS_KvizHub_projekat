using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Quiz
{
    public class SubmitAnswerDto
    {
        public int QuestionId { get; set; }
        public List<int> AnswerOptionIds { get; set; } = new();
        public string? AnswerText { get; set; }
    }
}