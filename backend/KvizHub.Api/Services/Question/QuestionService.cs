using KvizHub.Api.Data;
using KvizHub.Api.Dtos.Question;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Question
{
    public class QuestionService : IQuestionService
    {
        private readonly KvizHubContext _context;

        public QuestionService(KvizHubContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsForQuizAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizID == quizId && !q.IsArchived)
                .Include(q => q.AnswerOptions) 
                .Select(q => new QuestionDto
                {
                    QuestionID = q.QuestionID,
                    QuestionText = q.QuestionText,
                    PointNum = q.PointNum,
                    Type = q.Type.ToString(),
                    CorrectTextAnswer = q.CorrectTextAnswer,
                    AnswerOptions = q.AnswerOptions.Select(ao => new AnswerOptionDto
                    {
                        AnswerOptionID = ao.AnswerOptionID,
                        Text = ao.Text,
                        IsCorrect = ao.IsCorrect
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<QuestionDto?> AddQuestionToQuizAsync(int quizId, CreateQuestionDto dto)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null || quiz.IsArchived)
            {
                return null; 
            }

            var newQuestion = new Models.Question
            {
                QuizID = quizId,
                QuestionText = dto.QuestionText,
                PointNum = dto.PointNum,
                Type = dto.Type,
                CorrectTextAnswer = dto.CorrectTextAnswer ?? string.Empty
            };

            foreach (var answerDto in dto.AnswerOptions)
            {
                newQuestion.AnswerOptions.Add(new Models.AnswerOption
                {
                    Text = answerDto.Text,
                    IsCorrect = answerDto.IsCorrect
                });
            }

            await _context.Questions.AddAsync(newQuestion);
            await _context.SaveChangesAsync();

            var questionDto = new QuestionDto
            {
                QuestionID = newQuestion.QuestionID,
                QuestionText = newQuestion.QuestionText,
                PointNum = newQuestion.PointNum,
                Type = newQuestion.Type.ToString(),
                CorrectTextAnswer = newQuestion.CorrectTextAnswer,
                AnswerOptions = newQuestion.AnswerOptions.Select(ao => new AnswerOptionDto
                {
                    AnswerOptionID = ao.AnswerOptionID,
                    Text = ao.Text,
                    IsCorrect = ao.IsCorrect
                }).ToList()
            };

            return questionDto;
        }
    }
}