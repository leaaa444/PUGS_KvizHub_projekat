using KvizHub.Api.Services.LiveResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KvizHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LiveResultsController : ControllerBase
    {
        private readonly ILiveResultService _liveResultService;

        public LiveResultsController(ILiveResultService liveResultService)
        {
            _liveResultService = liveResultService;
        }

        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalLiveRankings([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var rankings = await _liveResultService.GetGlobalLiveRankingsAsync(startDate, endDate);
            return Ok(rankings);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetFinishedArenas()
        {
            var arenas = await _liveResultService.GetFinishedArenasAsync();
            return Ok(arenas);
        }

        [HttpGet("{roomCode}")] 
        public async Task<IActionResult> GetArenaDetails(string roomCode)
        {
            var arenaDetails = await _liveResultService.GetArenaDetailsAsync(roomCode);
            if (arenaDetails == null)
            {
                return NotFound("Arena sa datim kodom nije pronađena ili nije završena.");
            }
            return Ok(arenaDetails);
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyFinishedArenas()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var arenas = await _liveResultService.GetMyFinishedArenasAsync(userId);
            return Ok(arenas);
        }
    }
}
