using KvizHub.Api.Dtos.Quiz;
using KvizHub.Api.Dtos.Result;
using KvizHub.Api.Services;
using KvizHub.Api.Services.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KvizHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService _resultService;

        public ResultsController(IResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet("{resultId}")]
        public async Task<IActionResult> GetResultDetails(int resultId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _resultService.GetResultDetailsAsync(resultId, userId);

            if (result == null)
            {
                return NotFound("Rezultat nije pronađen ili nemate pristup.");
            }

            return Ok(result);
        }

        [HttpGet("my-results")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var results = await _resultService.GetMyResultsAsync(userId);
            return Ok(results);
        }

        [HttpGet("history/{resultId}")]
        public async Task<IActionResult> GetArchivedResultDetails(int resultId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _resultService.GetArchivedResultDetailsAsync(resultId, userId);

            if (result == null) return NotFound("Rezultat nije pronađen ili nemate pristup.");

            return Ok(result);
        }

        [HttpGet("progress/{quizId}")]
        public async Task<IActionResult> GetQuizProgress(int quizId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var progress = await _resultService.GetQuizProgressAsync(quizId, userId);
            return Ok(progress);
        }

        [HttpGet("all-rankings")]
        public async Task<IActionResult> GetAllRankings([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var allRankings = await _resultService.GetAllQuizRankingsAsync(startDate, endDate);
            return Ok(allRankings);
        }

        [HttpGet("global-ranking")]
        public async Task<IActionResult> GetGlobalRanking([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var rankings = await _resultService.GetGlobalRankingsAsync(startDate, endDate);
            return Ok(rankings);
        }

        [HttpGet("all-admin-results")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdminResults([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? username = null, [FromQuery] int? quizId = null)
        {
            var paginatedResults = await _resultService.GetAllResultsAsync(pageNumber, pageSize, username, quizId);
            return Ok(paginatedResults);
        }

        [HttpPost]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] QuizSubmissionDto submissionDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID is missing or invalid in the token.");
            }

            try
            {
                var result = await _resultService.SubmitQuizAsync(submissionDto, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Došlo je do interne greške prilikom obrade kviza.");
            }
        }

    }
}