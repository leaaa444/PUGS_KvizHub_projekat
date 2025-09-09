using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Quiz
{
    public class QuizSubmissionDto
    {
        public int QuizId { get; set; }
        public int TimeTaken { get; set; }
        public List<SubmitAnswerDto> Answers { get; set; } = new();
    }
}