using KvizHub.Api.Data;
using KvizHub.Api.Dtos.AnswerOption;
using KvizHub.Api.Dtos.Category;
using KvizHub.Api.Dtos.Question;
using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;
using KvizHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Api.Services.Quizzes
{
    public class QuizService : IQuizService
    {
        private readonly KvizHubContext _context;

        public QuizService(KvizHubContext context)
        {
            _context = context;
        }

        #region GET
        public async Task<IEnumerable<QuizListDto>> GetQuizzesAsync()
        {
            return await _context.Quizzes
                .Where(q => !q.IsArchived)
                .Include(q => q.QuizCategories)
                    .ThenInclude(qc => qc.Category)
                .Include(q => q.Questions)
                .OrderByDescending(q => q.QuizResults.Count())
                .Select(q => new QuizListDto
                {
                    QuizID = q.QuizID,
                    Name = q.Name,
                    Description = q.Description,
                    Difficulty = q.Difficulty.ToString(),
                    TimesCompleted = q.QuizResults.Count(),
                    MaxPoints = q.Questions.Sum(p => p.PointNum),
                    NumberOfQuestions = q.Questions.Count(),
                    TimeLimit = q.TimeLimit,
                    Categories = q.QuizCategories.Select(qc => qc.Category.Name).ToList()
                }).ToListAsync();
        }

        public async Task<QuizDto?> GetQuizByIdAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.QuizCategories)
                .ThenInclude(qc => qc.Category)
                .FirstOrDefaultAsync(q => q.QuizID == quizId && !q.IsArchived);

            if (quiz == null)
            {
                return null;
            }

            var quizDto = new QuizDto
            {
                QuizID = quiz.QuizID,
                Name = quiz.Name,
                Description = quiz.Description,
                Difficulty = quiz.Difficulty.ToString(),
                TimeLimit = quiz.TimeLimit,
                Categories = quiz.QuizCategories.Select(qc => new CategoryDto
                {
                    CategoryID = qc.Category.CategoryID,
                    Name = qc.Category.Name
                }).ToList()
            };

            return quizDto;

        }

        public async Task<QuizForTakerDto> GetQuizForTakerAsync(int quizId)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.QuizID == quizId && !q.IsArchived)
                .Include(q => q.Questions)
                    .ThenInclude(p => p.AnswerOptions)
                .Select(q => new QuizForTakerDto
                {
                    QuizId = q.QuizID,
                    Name = q.Name,
                    TimeLimit = q.TimeLimit,
                    Questions = q.Questions.Select(p => new QuestionForQuizTakerDto
                    {
                        QuestionId = p.QuestionID,
                        QuestionText = p.QuestionText,
                        Type = p.Type,
                        PointNum = p.PointNum,
                        AnswerOptions = p.AnswerOptions.Select(ao => new AnswerOptionForQuizTakerDto
                        {
                            AnswerOptionId = ao.AnswerOptionID,
                            Text = ao.Text
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return quiz;
        }

        #endregion

        public async Task<QuizDto?> CreateQuiz(CreateQuizWithQuestionsDto dto)
        {
            if (await _context.Quizzes.AnyAsync(q => q.Name.ToLower() == dto.Name.ToLower() && !q.IsArchived))
            {
                return null; 
            }

            var newQuiz = new Quiz
            {
                Name = dto.Name,
                Description = dto.Description,
                Difficulty = dto.Difficulty,
                TimeLimit = dto.TimeLimit
            };

            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.CategoryID)).ToListAsync();
            foreach (var category in categories)
            {
                newQuiz.QuizCategories.Add(new QuizCategory { Category = category });
            }

            foreach (var questionDto in dto.Questions)
            {
                var newQuestion = new Models.Question
                {
                    QuestionText = questionDto.QuestionText,
                    Type = questionDto.Type,
                    PointNum = questionDto.PointNum,
                    CorrectTextAnswer = questionDto.CorrectTextAnswer ?? string.Empty,
                    Quiz = newQuiz 
                };

                foreach (var answerDto in questionDto.AnswerOptions)
                {
                    newQuestion.AnswerOptions.Add(new Models.AnswerOption
                    {
                        Text = answerDto.Text,
                        IsCorrect = answerDto.IsCorrect
                    });
                }
                newQuiz.Questions.Add(newQuestion);
            }

            await _context.Quizzes.AddAsync(newQuiz);
            await _context.SaveChangesAsync();

            return new QuizDto 
            {  
                QuizID = newQuiz.QuizID,
                Name = newQuiz.Name,
                Description = newQuiz.Description,
                Difficulty = newQuiz.Difficulty.ToString(),
                TimeLimit = newQuiz.TimeLimit,
                Categories = newQuiz.QuizCategories.Select(qc => new CategoryDto
                {
                    CategoryID = qc.Category.CategoryID,
                    Name = qc.Category.Name
                }).ToList()
            };
        }

        #region UPDATE
        public async Task<QuizDto?> UpdateQuizAsync(int quizId, UpdateQuizDto updateQuizDto)
        {
            var quiz = await _context.Quizzes
            .Include(q => q.QuizCategories)
                .ThenInclude(qc => qc.Category)
            .Include(q => q.Questions)
                .ThenInclude(qu => qu.AnswerOptions)
            .FirstOrDefaultAsync(q => q.QuizID == quizId);

            if (quiz == null || quiz.IsArchived) return null;

            UpdateQuizDetails(quiz, updateQuizDto);
            await UpdateQuizCategories(quiz, updateQuizDto);
            UpdateQuizQuestions(quiz, updateQuizDto);

            await _context.SaveChangesAsync();

            return new QuizDto
            {
                QuizID = quiz.QuizID,
                Name = quiz.Name,
                Description = quiz.Description,
                Difficulty = quiz.Difficulty.ToString(),
                TimeLimit = quiz.TimeLimit,
                Categories = quiz.QuizCategories.Select(c => new CategoryDto { CategoryID = c.CategoryID, Name = c.Category.Name }).ToList()
            };
        }

        private void UpdateQuizDetails(Quiz quiz, UpdateQuizDto dto)
        {
            quiz.Name = dto.Name;
            quiz.Description = dto.Description;
            quiz.Difficulty = dto.Difficulty;
            quiz.TimeLimit = dto.TimeLimit;
        }

        private async Task UpdateQuizCategories(Quiz quiz, UpdateQuizDto dto)
        {
            _context.QuizCategories.RemoveRange(quiz.QuizCategories);

            await _context.SaveChangesAsync();

            var categories = await _context.Categories
                .Where(c => dto.CategoryIds.Contains(c.CategoryID))
                .ToListAsync();

            foreach (var category in categories)
            {
                quiz.QuizCategories.Add(new QuizCategory { Category = category });
            }
        }

        private void UpdateQuizQuestions(Quiz quiz, UpdateQuizDto dto)
        {
            var questionIdsFromDto = dto.Questions.Where(q => q.QuestionID > 0).Select(q => q.QuestionID).ToList();


            var questionsToDelete = quiz.Questions
                .Where(dbQ => !questionIdsFromDto.Contains(dbQ.QuestionID))
                .ToList();
            if (questionsToDelete.Any())
            {
                _context.Questions.RemoveRange(questionsToDelete);
            }

            foreach (var questionDto in dto.Questions)
            {
                if (questionDto.QuestionID <= 0)
                {
                    var newQuestion = new Models.Question
                    {
                        QuestionText = questionDto.QuestionText,
                        Type = questionDto.Type,
                        PointNum = questionDto.PointNum,
                        CorrectTextAnswer = questionDto.CorrectTextAnswer ?? string.Empty
                    };

                    foreach (var answerDto in questionDto.AnswerOptions)
                    {
                        newQuestion.AnswerOptions.Add(new Models.AnswerOption
                        {
                            Text = answerDto.Text,
                            IsCorrect = answerDto.IsCorrect
                        });
                    }
                    quiz.Questions.Add(newQuestion);
                }
                else
                {
                    var dbQuestion = quiz.Questions.FirstOrDefault(q => q.QuestionID == questionDto.QuestionID);
                    if (dbQuestion != null)
                    {
                        dbQuestion.QuestionText = questionDto.QuestionText;
                        dbQuestion.Type = questionDto.Type;
                        dbQuestion.PointNum = questionDto.PointNum;
                        dbQuestion.CorrectTextAnswer = questionDto.CorrectTextAnswer ?? string.Empty;

                        UpdateAnswerOptions(dbQuestion, questionDto.AnswerOptions);
                    }
                }
            }
        }

        private void UpdateAnswerOptions(Models.Question dbQuestion, List<UpdateAnswerOptionDto> answerDtos)
        {
            var answerIdsFromDto = answerDtos.Where(a => a.AnswerOptionID > 0).Select(a => a.AnswerOptionID).ToList();

            var answersToDelete = dbQuestion.AnswerOptions
                .Where(dbAns => dbAns.AnswerOptionID > 0 && !answerIdsFromDto.Contains(dbAns.AnswerOptionID))
                .ToList();
            if (answersToDelete.Any())
            {
                _context.AnswerOptions.RemoveRange(answersToDelete);
            }

            foreach (var answerDto in answerDtos)
            {
                if (answerDto.AnswerOptionID <= 0) 
                {
                    dbQuestion.AnswerOptions.Add(new Models.AnswerOption
                    {
                        Text = answerDto.Text,
                        IsCorrect = answerDto.IsCorrect
                    });
                }
                else 
                {
                    var dbAnswer = dbQuestion.AnswerOptions.FirstOrDefault(ans => ans.AnswerOptionID == answerDto.AnswerOptionID);
                    if (dbAnswer != null)
                    {
                        dbAnswer.Text = answerDto.Text;
                        dbAnswer.IsCorrect = answerDto.IsCorrect;
                    }
                }
            }
        }
        #endregion

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
            {
                return false;
            }

            quiz.IsArchived = true; 
            await _context.SaveChangesAsync();
            return true;
        }

    }
}