using KvizHub.Api.Dtos.Question;
using KvizHub.Api.Models;
using System.Threading.Tasks;

namespace KvizHub.Api.Services.Question
{
    public interface IQuestionService
    {
        Task<QuestionDto?> AddQuestionToQuizAsync(int quizId, CreateQuestionDto dto);

        Task<IEnumerable<QuestionDto>> GetQuestionsForQuizAsync(int quizId);

    }
}