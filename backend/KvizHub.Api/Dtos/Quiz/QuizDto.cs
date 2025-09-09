using KvizHub.Api.Dtos.Category;
using System.Collections.Generic;

namespace KvizHub.Api.Dtos.Quiz
{
    public class QuizDto
    {
        public int QuizID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int TimeLimit { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }
}