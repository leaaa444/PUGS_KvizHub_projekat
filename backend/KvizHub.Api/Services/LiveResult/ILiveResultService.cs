using KvizHub.Api.Dtos.GameRoom;
using KvizHub.Api.Dtos.LiveResult;

namespace KvizHub.Api.Services.LiveResult
{
    public interface ILiveResultService
    {
        Task<IEnumerable<LiveGlobalRankingDto>> GetGlobalLiveRankingsAsync(DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<FinishedArenaDto>> GetFinishedArenasAsync();

        Task<GameRoomDto?> GetArenaDetailsAsync(string roomCode);

        Task<IEnumerable<MyFinishedArenaDto>> GetMyFinishedArenasAsync(int userId);
    }
}
