using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;
using KvizHub.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Quizzes
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizListDto>> GetSoloQuizzesAsync();
        Task<IEnumerable<QuizListDto>> GetLiveQuizzesAsync();
        Task<QuizDto?> GetQuizByIdAsync(int quizId);
        Task<QuizForTakerDto> GetQuizForTakerAsync(int quizId);
        Task<QuizDto> CreateQuiz(CreateQuizWithQuestionsDto dto);
        Task<QuizDto?> UpdateQuizAsync(int quizId, UpdateQuizDto updateQuizDto);
        Task<QuizDto> ArchiveAndCreateNewAsync(int originalQuizId, CreateQuizWithQuestionsDto createDto);
        Task<bool> DeleteQuizAsync(int quizId);
    }
}