using KvizHub.Api.Dtos.Question; 
using KvizHub.Api.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KvizHub.Api.Dtos.Quiz
{
    public class CreateQuizWithQuestionsDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Naziv mora imati bar 3 karaktera.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public QuizDifficulty Difficulty { get; set; }

        public QuizMode Mode { get; set; }

        public int TimeLimit { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Kviz mora imati bar jednu kategoriju.")]
        public List<int> CategoryIds { get; set; } = new();

        public List<CreateQuestionDto> Questions { get; set; } = new();
    }
}