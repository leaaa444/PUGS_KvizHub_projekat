using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;

namespace KvizHub.Api.Services
{
    public interface IResultService
    {
        Task<QuizResultDetailsDto?> GetResultDetailsAsync(int resultId, string userId);

        Task<QuizResultDto> SubmitQuizAsync(QuizSubmissionDto submissionDto, int userId);

    }
}