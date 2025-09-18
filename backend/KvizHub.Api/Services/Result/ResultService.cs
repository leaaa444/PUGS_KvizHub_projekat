using KvizHub.Api.Data;
using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;
using KvizHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Api.Services.Result
{
    public class ResultService : IResultService
    {
        private readonly KvizHubContext _context;

        public ResultService(KvizHubContext context)
        {
            _context = context;
        }

        #region get

        public async Task<QuizResultDetailsDto?> GetResultDetailsAsync(int resultId, string userId)
        {
            var resultEntity = await _context.QuizResults
                .Where(qr => qr.QuizResultID == resultId && qr.User.UserID.ToString() == userId)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qu => qu.AnswerOptions)
                .Include(qr => qr.UserAnswers)
                    .ThenInclude(ua => ua.SelectedOptions)
                .FirstOrDefaultAsync();

            if (resultEntity == null)
            {
                return null;
            }


            var resultDto = new QuizResultDetailsDto
            {
                ResultId = resultEntity.QuizResultID,
                QuizName = resultEntity.Quiz.Name,
                Score = resultEntity.Score,
                MaxPossibleScore = resultEntity.Quiz.Questions.Sum(q => q.PointNum),
                DateCompleted = resultEntity.DateOfCompletion,
                TimeTaken = resultEntity.CompletionTime,
                Questions = resultEntity.Quiz.Questions.Select(q =>
                {
                    var userAnswer = resultEntity.UserAnswers.FirstOrDefault(ua => ua.QuestionID == q.QuestionID);

                    return new QuestionResultDto
                    {
                        QuestionId = q.QuestionID,
                        Text = q.QuestionText,
                        Type = q.Type,
                        Options = q.AnswerOptions.Select(opt => new OptionResultDto
                        {
                            OptionId = opt.AnswerOptionID,
                            Text = opt.Text
                        }).ToList(),
                        CorrectAnswer = new CorrectAnswerResultDto
                        {
                            AnswerOptionIds = q.AnswerOptions.Where(opt => opt.IsCorrect).Select(opt => opt.AnswerOptionID).ToList(),
                            AnswerText = q.CorrectTextAnswer
                        },
                        UserAnswer = userAnswer != null ? new UserAnswerResultDto
                        {
                            AnswerText = userAnswer.GivenTextAnswer,
                            AnswerOptionIds = userAnswer.SelectedOptions.Select(so => so.AnswerOptionId).ToList()
                        } : new UserAnswerResultDto(), // Ako nema odgovora, vrati prazan objekat
                                                       // Sada bezbedno pozivamo našu C# metodu
                        IsCorrect = userAnswer != null && IsAnswerCorrect(userAnswer, q)
                    };
                }).ToList()
            };

            return resultDto;
        }

        private static bool IsAnswerCorrect(Models.UserAnswer userAnswer, Models.Question question)
        {
            if (question.Type == Models.Enums.QuestionType.FillInTheBlank)
            {
                return userAnswer.GivenTextAnswer?.Trim().Equals(question.CorrectTextAnswer?.Trim(), StringComparison.OrdinalIgnoreCase) ?? false;
            }
            else
            {
                var correctOptionIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.AnswerOptionID).ToHashSet();
                var selectedOptionIds = userAnswer.SelectedOptions.Select(so => so.AnswerOptionId).ToHashSet();
                return correctOptionIds.SetEquals(selectedOptionIds);
            }
        }

        public async Task<IEnumerable<MyResultDto>> GetMyResultsAsync(string userId)
        {
            var results = await _context.QuizResults
                .Where(qr => qr.User.UserID.ToString() == userId)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Questions)
                .OrderByDescending(qr => qr.DateOfCompletion)
                .Select(qr => new MyResultDto
                {
                    ResultId = qr.QuizResultID,
                    QuizName = qr.Quiz.Name,
                    DateCompleted = qr.DateOfCompletion,
                    Score = qr.Score,
                    Percentage = qr.Quiz.Questions.Any() ? (qr.Score / qr.Quiz.Questions.Sum(q => q.PointNum)) * 100 : 0,
                    AttemptNum = qr.AttemptNum
                })
                .ToListAsync();

            return results;
        }

        public async Task<ArchivedResultDetailsDto?> GetArchivedResultDetailsAsync(int resultId, string userId)
        {
            var result = await _context.QuizResults
                .Where(qr => qr.QuizResultID == resultId && qr.User.UserID.ToString() == userId)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qu => qu.AnswerOptions)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.QuizCategories)
                        .ThenInclude(qc => qc.Category)
                .Include(qr => qr.UserAnswers)
                    .ThenInclude(ua => ua.SelectedOptions)
                .Select(qr => new ArchivedResultDetailsDto
                {
                    QuizId = qr.QuizID,
                    QuizName = qr.Quiz.Name,
                    Description = qr.Quiz.Description,
                    Difficulty = qr.Quiz.Difficulty.ToString(),
                    Categories = qr.Quiz.QuizCategories.Select(qc => qc.Category.Name).ToList(),
                    TimeTaken = qr.CompletionTime,
                    Questions = qr.Quiz.Questions.Select(q => new ArchivedQuestionDto
                    {
                        QuestionId = q.QuestionID,
                        Type = q.Type,
                        Text = q.QuestionText,
                        Points = q.PointNum,
                        Options = q.AnswerOptions.Select(opt => new OptionResultDto
                        {
                            OptionId = opt.AnswerOptionID,
                            Text = opt.Text
                        }).ToList(),
                        UserAnswer = qr.UserAnswers.Where(ua => ua.QuestionID == q.QuestionID)
                                       .Select(ua => new UserAnswerResultDto
                                       {
                                           AnswerText = ua.GivenTextAnswer,
                                           AnswerOptionIds = ua.SelectedOptions.Select(so => so.AnswerOptionId).ToList()
                                       }).FirstOrDefault() ?? new UserAnswerResultDto()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<QuizProgressDto>> GetQuizProgressAsync(int quizId, string userId)
        {
            var progress = await _context.QuizResults
                .Where(qr => qr.User.UserID.ToString() == userId && qr.QuizID == quizId)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Questions)
                .OrderBy(qr => qr.AttemptNum) 
                .Select(qr => new QuizProgressDto
                {
                    AttemptNum = qr.AttemptNum,
                    Percentage = qr.Quiz.Questions.Any() ? (qr.Score / qr.Quiz.Questions.Sum(q => q.PointNum)) * 100 : 0
                })
                .ToListAsync();

            return progress;
        }

        public async Task<IEnumerable<AllRankingsDto>> GetAllQuizRankingsAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && !endDate.HasValue)
            {
                endDate = startDate;
            }

            IQueryable<QuizResult> resultsQuery = _context.QuizResults.AsQueryable();

            if (startDate.HasValue)
            {
                resultsQuery = resultsQuery.Where(qr => qr.DateOfCompletion.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                resultsQuery = resultsQuery.Where(qr => qr.DateOfCompletion.Date <= endDate.Value.Date);
            }

            var bestResults = await resultsQuery
                .Include(qr => qr.User)    
                .Include(qr => qr.Quiz)    
                .GroupBy(qr => new { qr.UserID, qr.QuizID })
                .Select(group => group.OrderByDescending(qr => qr.Score).ThenBy(qr => qr.CompletionTime).First())
                .ToListAsync();

            var allRankings = new List<AllRankingsDto>();

            var rankingsByQuiz = bestResults
                 .GroupBy(r => r.Quiz)
                 .Select(group => new AllRankingsDto
                 {
                     QuizId = group.Key.QuizID,
                     QuizName = group.Key.Name,
                     TopPlayers = group
                         .OrderByDescending(r => r.Score) 
                         .ThenBy(r => r.CompletionTime)
                         .Select(r => new RankingEntryDto
                         {
                             Username = r.User.Username,
                             UserProfilePictureUrl = r.User.ProfilePictureUrl,
                             Score = r.Score,
                             TimeTaken = r.CompletionTime,
                             DateCompleted = r.DateOfCompletion
                         }).ToList()
                 })
                 .OrderBy(q => q.QuizName) 
                 .ToList();

            return rankingsByQuiz;
        }

        public async Task<IEnumerable<GlobalRankingDto>> GetGlobalRankingsAsync()
        {
            var allResults = await _context.QuizResults
                .Include(qr => qr.User)
                .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Questions)
                .ToListAsync();

            var userScores = new Dictionary<int, List<double>>();

            var bestUserResults = allResults
                .GroupBy(r => new { r.UserID, r.QuizID })
                .Select(g => g.OrderByDescending(r => r.Score).ThenBy(r => r.CompletionTime).First());

            var resultsByUser = bestUserResults.GroupBy(r => r.User);

            var globalScores = new List<(User User, double GlobalScore, int QuizzesPlayed)>();

            foreach (var userGroup in resultsByUser)
            {
                var user = userGroup.Key;
                var weightedScores = new List<double>();

                foreach (var result in userGroup)
                {
                    var maxPoints = result.Quiz.Questions.Sum(q => q.PointNum);
                    if (maxPoints == 0) continue;

                    double percentage = (result.Score / maxPoints) * 100;
                    double weight = result.Quiz.Difficulty switch
                    {
                        Models.Enums.QuizDifficulty.Medium => 1.25,
                        Models.Enums.QuizDifficulty.Hard => 1.5,
                        _ => 1.0
                    };

                    weightedScores.Add(percentage * weight);
                }

                if (weightedScores.Any())
                {
                    globalScores.Add((user, weightedScores.Average(), weightedScores.Count));
                }
            }

            var rankedUsers = globalScores
                .OrderByDescending(s => s.GlobalScore)
                .Select((s, index) => new GlobalRankingDto
                {
                    Position = index + 1,
                    Username = s.User.Username,
                    UserProfilePictureUrl = s.User.ProfilePictureUrl,
                    GlobalScore = s.GlobalScore,
                    QuizzesPlayed = s.QuizzesPlayed
                });

            return rankedUsers;
        }

        public async Task<PaginatedResult<AdminResultDto>> GetAllResultsAsync(int pageNumber, int pageSize, string? username, int? quizId)
        {
            var query = _context.QuizResults
                                .Include(qr => qr.User)
                                .Include(qr => qr.Quiz)
                                .AsQueryable();

            if (quizId.HasValue)
            {
                query = query.Where(qr => qr.QuizID == quizId.Value);
            }
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(qr => qr.User.Username.Contains(username));
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .OrderByDescending(qr => qr.DateOfCompletion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(qr => new AdminResultDto
                {
                    ResultId = qr.QuizResultID,
                    QuizName = qr.Quiz.Name,
                    Username = qr.User.Username,
                    Score = qr.Score,
                    CompletionTime = qr.CompletionTime,
                    DateOfCompletion = qr.DateOfCompletion
                })
                .ToListAsync();

            return new PaginatedResult<AdminResultDto>
            {
                Items = results,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };
        }

        #endregion

        public async Task<QuizResultDto> SubmitQuizAsync(QuizSubmissionDto submissionDto, int userId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(p => p.AnswerOptions)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuizID == submissionDto.QuizId);

            if (quiz == null)
            {
                throw new KeyNotFoundException("Kviz nije pronađen.");
            }

            double totalScore = 0;
            int correctAnswersCount = 0;
            var userAnswersToSave = new List<UserAnswer>();

            foreach (var userAnswerDto in submissionDto.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.QuestionID == userAnswerDto.QuestionId);
                if (question == null) continue;

                bool isCorrect = false;

                var userAnswer = new UserAnswer
                {
                    QuestionID = question.QuestionID,
                };

                switch (question.Type)
                {
                    case Models.Enums.QuestionType.SingleChoice:
                    case Models.Enums.QuestionType.TrueFalse:
                        var correctOptionId = question.AnswerOptions.FirstOrDefault(o => o.IsCorrect)?.AnswerOptionID;
                        var userAnswerId = userAnswerDto.AnswerOptionIds.FirstOrDefault();

                        isCorrect = userAnswerId == correctOptionId;
                        if (userAnswerId > 0)
                        {
                            userAnswer.SelectedOptions.Add(new UserAnswerSelectedOption { AnswerOptionId = userAnswerId });
                        }
                        break;

                    case Models.Enums.QuestionType.MultipleChoice:
                        var correctOptionIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.AnswerOptionID).ToHashSet();
                        var userOptionIds = userAnswerDto.AnswerOptionIds.ToHashSet();

                        isCorrect = correctOptionIds.SetEquals(userOptionIds);
                        userAnswer.SelectedOptions = userAnswerDto.AnswerOptionIds
                           .Select(id => new UserAnswerSelectedOption { AnswerOptionId = id })
                           .ToList();
                        break;

                    case Models.Enums.QuestionType.FillInTheBlank:
                        isCorrect = question.CorrectTextAnswer.Trim().Equals(userAnswerDto.AnswerText?.Trim(), StringComparison.OrdinalIgnoreCase);
                        userAnswer.GivenTextAnswer = userAnswerDto.AnswerText ?? string.Empty;
                        break;
                }

                if (isCorrect)
                {
                    totalScore += question.PointNum;
                    correctAnswersCount++;
                }

                userAnswersToSave.Add(userAnswer);
            }

            int attemptNum = await _context.QuizResults
                .CountAsync(r => r.QuizID == quiz.QuizID && r.UserID == userId) + 1;

            var quizResult = new QuizResult
            {
                QuizID = quiz.QuizID,
                UserID = userId,
                Score = totalScore,
                CorrectAnswers = correctAnswersCount,
                DateOfCompletion = DateTime.UtcNow,
                UserAnswers = userAnswersToSave,
                AttemptNum = attemptNum,
                CompletionTime = submissionDto.TimeTaken,
            };
            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();

            return new QuizResultDto
            {
                ResultId = quizResult.QuizResultID,
                QuizId = quiz.QuizID,
                Score = quizResult.Score,
                MaxPossibleScore = quiz.Questions.Sum(q => q.PointNum),
                CorrectAnswers = correctAnswersCount
            };
        }

    }
}