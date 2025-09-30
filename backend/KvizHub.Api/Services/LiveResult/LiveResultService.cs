using KvizHub.Api.Data;
using KvizHub.Api.Dtos.GameRoom;
using KvizHub.Api.Dtos.LiveResult;
using KvizHub.Api.Dtos.Player;
using KvizHub.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace KvizHub.Api.Services.LiveResult
{
    public class LiveResultService : ILiveResultService
    {
        private readonly KvizHubContext _context;

        public LiveResultService(KvizHubContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LiveGlobalRankingDto>> GetGlobalLiveRankingsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.LiveQuizParticipants.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(p => p.GameRoom.FinishedAt.HasValue && p.GameRoom.FinishedAt.Value.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                query = query.Where(p => p.GameRoom.FinishedAt.HasValue && p.GameRoom.FinishedAt.Value.Date <= endDate.Value.Date);
            }

            var participantResults = await query
                .Include(p => p.User)
                .Include(p => p.GameRoom.Quiz) 
                .Where(p => p.GameRoom.Status == GameStatus.Finished && p.User != null)
                .ToListAsync();

            var rankings = participantResults
                .GroupBy(p => p.User)
                .Select(userGroup =>
                {
                    var weightedScores = userGroup.Select(p =>
                    {
                        double weight = p.GameRoom.Quiz.Difficulty switch
                        {
                            Models.Enums.QuizDifficulty.Medium => 1.25,
                            Models.Enums.QuizDifficulty.Hard => 1.5,
                            _ => 1.0
                        };
                        return p.Score * weight;
                    });

                    return new
                    {
                        User = userGroup.Key,
                        AverageScore = weightedScores.Any() ? weightedScores.Average() : 0,
                        ArenasPlayed = userGroup.Count()
                    };
                })
                .OrderByDescending(r => r.AverageScore)
                .ToList();

            return rankings.Select((r, index) => new LiveGlobalRankingDto
            {
                Position = index + 1,
                Username = r.User.Username,
                UserProfilePictureUrl = r.User.ProfilePictureUrl,
                AverageScore = r.AverageScore,
                ArenasPlayed = r.ArenasPlayed
            });
        }

        public async Task<IEnumerable<FinishedArenaDto>> GetFinishedArenasAsync()
        {
            var finishedArenas = await _context.GameRooms
                .Where(gr => gr.Status == Models.Enums.GameStatus.Finished && gr.FinishedAt.HasValue)
                .Include(gr => gr.Quiz)
                .Include(gr => gr.Participants)
                    .ThenInclude(p => p.User)
                .OrderByDescending(gr => gr.FinishedAt)
                .Select(gr => new
                {
                    Winner = gr.Participants.OrderByDescending(p => p.Score).FirstOrDefault(),
                    GameRoom = gr
                })
                .Select(temp => new FinishedArenaDto
                {
                    RoomCode = temp.GameRoom.RoomCode,
                    QuizName = temp.GameRoom.Quiz.Name,
                    HostUsername = temp.GameRoom.HostUsername,
                    FinishedAt = temp.GameRoom.FinishedAt!.Value,
                    ParticipantCount = temp.GameRoom.Participants.Count,
                    WinnerUsername = temp.Winner != null ? temp.Winner.User.Username : "N/A",
                    WinnerScore = temp.Winner != null ? temp.Winner.Score : 0
                })
                .ToListAsync();

            return finishedArenas;
        }

        public async Task<GameRoomDto?> GetArenaDetailsAsync(string roomCode)
        {
            var room = await _context.GameRooms
                .Include(gr => gr.Quiz)
                .Include(gr => gr.Participants).ThenInclude(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(gr => gr.RoomCode == roomCode && gr.Status == Models.Enums.GameStatus.Finished);

            if (room == null)
            {
                return null;
            }

            return new GameRoomDto
            {
                RoomCode = room.RoomCode,
                QuizId = room.QuizId,
                QuizName = room.Quiz?.Name ?? "Nepoznat kviz",
                HostUsername = room.HostUsername ?? "N/A",
                Status = room.Status.ToString(),
                CurrentQuestionIndex = room.CurrentQuestionIndex,
                CurrentQuestionStartTime = room.CurrentQuestionStartTime,
                Players = room.Participants
                    .Where(p => p.User != null)
                    .Select(p => new PlayerDto
                    {
                        Username = p.User!.Username ?? "Nepoznat igrač",
                        Score = p.Score,
                        ImageUrl = p.User!.ProfilePictureUrl,
                        IsDisconnected = p.DisconnectedAt.HasValue
                    }).ToList()
            };
        }

        public async Task<IEnumerable<MyFinishedArenaDto>> GetMyFinishedArenasAsync(int userId)
        {
            var finishedArenas = await _context.GameRooms
                .Where(gr =>
                    gr.Status == GameStatus.Finished &&
                    gr.FinishedAt.HasValue &&
                    gr.Participants.Any(p => p.UserId == userId))
                .Include(gr => gr.Quiz)
                .Include(gr => gr.Participants).ThenInclude(p => p.User)
                .OrderByDescending(gr => gr.FinishedAt)
                .ToListAsync();

            return finishedArenas.Select(gr =>
            {
                var myParticipant = gr.Participants.First(p => p.UserId == userId);
                var rank = gr.Participants.OrderByDescending(p => p.Score).ToList().FindIndex(p => p.UserId == userId) + 1;

                return new MyFinishedArenaDto
                {
                    RoomCode = gr.RoomCode,
                    QuizName = gr.Quiz.Name,
                    FinishedAt = gr.FinishedAt!.Value,
                    YourScore = myParticipant.Score,
                    YourRank = rank,
                    ParticipantCount = gr.Participants.Count
                };
            });
        }
    }
}
