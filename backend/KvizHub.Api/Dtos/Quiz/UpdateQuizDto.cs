using KvizHub.Api.Dtos.Question;
using KvizHub.Api.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Quiz
{
    public class UpdateQuizDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public QuizDifficulty Difficulty { get; set; }
        public int TimeLimit { get; set; }
        [Required]
        [MinLength(1)]
        public List<int> CategoryIds { get; set; } = new();
        public List<UpdateQuestionDto> Questions { get; set; } = new();
    }
}