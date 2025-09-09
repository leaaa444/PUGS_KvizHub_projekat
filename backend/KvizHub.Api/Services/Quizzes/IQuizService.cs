using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.QuizResult;
using KvizHub.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Quizzes
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizListDto>> GetQuizzesAsync();
        Task<QuizDto?> GetQuizByIdAsync(int quizId);
        Task<QuizForTakerDto> GetQuizForTakerAsync(int quizId);
        Task<QuizDto?> CreateQuiz(CreateQuizWithQuestionsDto dto);
        Task<QuizDto?> UpdateQuizAsync(int quizId, UpdateQuizDto updateQuizDto); 
        Task<bool> DeleteQuizAsync(int quizId);
        Task<QuizResultDto> SubmitQuizAsync(QuizSubmissionDto submissionDto, int userId);
    }
}