using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;

namespace KvizHub.Api.Services.Result
{
    public interface IResultService
    {
        Task<QuizResultDetailsDto?> GetResultDetailsAsync(int resultId, string userId);

        Task<IEnumerable<MyResultDto>> GetMyResultsAsync(string userId);

        Task<ArchivedResultDetailsDto?> GetArchivedResultDetailsAsync(int resultId, string userId);

        Task<IEnumerable<QuizProgressDto>> GetQuizProgressAsync(int quizId, string userId);

        Task<IEnumerable<AllRankingsDto>> GetAllQuizRankingsAsync(DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<GlobalRankingDto>> GetGlobalRankingsAsync();

        Task<PaginatedResult<AdminResultDto>> GetAllResultsAsync(int pageNumber, int pageSize, string? username, int? quizId);

        Task<QuizResultDto> SubmitQuizAsync(QuizSubmissionDto submissionDto, int userId);


    }
}