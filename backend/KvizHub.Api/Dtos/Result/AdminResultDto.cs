namespace KvizHub.Api.Dtos.Result
{
    public class AdminResultDto
    {
        public int ResultId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public double Score { get; set; }
        public int CompletionTime { get; set; }
        public DateTime DateOfCompletion { get; set; }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
