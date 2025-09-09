using KvizHub.Api.Dtos.Question;
using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Quiz
{
    public class QuizForTakerDto
    {
        public int QuizId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TimeLimit { get; set; }
        public List<QuestionForQuizTakerDto> Questions { get; set; } = new();
    }
}