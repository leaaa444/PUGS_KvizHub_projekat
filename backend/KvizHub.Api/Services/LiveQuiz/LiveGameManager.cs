using KvizHub.Api.Data;
using KvizHub.Api.Dtos.GameRoom;
using KvizHub.Api.Dtos.Player;
using KvizHub.Api.Dtos.Question;
using KvizHub.Api.Hubs;
using KvizHub.Api.Models;
using KvizHub.Api.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace KvizHub.Api.Services.LiveQuiz
{
    public class LiveGameManager : ILiveGameManager
    {
        // SVA STANJA SU PREMEŠTENA OVDE IZ HUBA
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
        private static readonly ConcurrentDictionary<string, bool> _advancingRooms = new();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _roomTimers = new();

        private readonly IHubContext<QuizHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LiveGameManager> _logger;

        public LiveGameManager(IHubContext<QuizHub> hubContext, IServiceScopeFactory scopeFactory, ILogger<LiveGameManager> logger)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void UserConnected(string connectionId, string username)
        {
            _userConnections[connectionId] = username;
        }

        public async Task UserDisconnectedAsync(string connectionId)
        {
            if (_userConnections.TryRemove(connectionId, out var username))
            {
                await HandleDisconnect(username);
            }
        }

        #region Lobby Management
        public async Task CreateRoomAsync(int quizId, string hostUsername, string connectionId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var roomCode = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
            var newRoomDb = new GameRoom { RoomCode = roomCode, QuizId = quizId, HostUsername = hostUsername, Status = GameStatus.Lobby };

            context.GameRooms.Add(newRoomDb);
            await context.SaveChangesAsync();
            _logger.LogInformation($"[LiveGameManager] Soba '{roomCode}' kreirana.");

            await _hubContext.Groups.AddToGroupAsync(connectionId, roomCode);
            await _hubContext.Clients.Client(connectionId).SendAsync("RoomCreated", roomCode);
        }

        public async Task<(GameRoomDto? room, bool isNewParticipant, bool gameInProgress)> EnterLobbyAsync(string roomCode, string username, int userId, string connectionId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var room = await context.GameRooms
                .Include(r => r.Quiz)
                .Include(r => r.Participants).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode);

            if (room == null || room.Status == GameStatus.Finished)
            {
                return (null, false, false);
            }

            bool isNewParticipant = false;
            var participant = room.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
            {
                if (room.HostUsername != username)
                {
                    room.Participants.Add(new LiveQuizParticipant { UserId = userId, Score = 0 });
                    isNewParticipant = true;
                }
            }
            else
            {
                if (participant.DisconnectedAt.HasValue)
                {
                    _logger.LogInformation($"[LiveGameManager] Igrač '{username}' se rekonektovao u sobu '{roomCode}'. Vraćam ga u igru.");
                    participant.DisconnectedAt = null; 
                }
            }
            await context.SaveChangesAsync();

            var finalRoomState = await context.GameRooms
                .Include(r => r.Quiz)
                .Include(r => r.Participants).ThenInclude(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode);

            return (MapRoomToDto(finalRoomState), isNewParticipant, room.Status == GameStatus.InProgress);
        }
        #endregion

        #region Game Flow
        public async Task StartGameAsync(string roomCode, string hostUsername)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var room = await context.GameRooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room == null || room.HostUsername != hostUsername) return;

            room.Status = GameStatus.InProgress;
            await context.SaveChangesAsync();
            await _hubContext.Clients.Group(roomCode).SendAsync("GameStarted");

            await SendQuestion(room.Id);
        }

        public async Task SubmitAnswerAsync(SubmitAnswerDto answerDto, string userIdStr, string connectionId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            _logger.LogInformation($"[LiveGameManager] Odgovor primljen za sobu: {answerDto.RoomCode}, PitanjeId: {answerDto.QuestionId}");

            try
            {
                var room = await context.GameRooms
                    .Include(r => r.Quiz)
                    .Include(r => r.Participants)
                    .FirstOrDefaultAsync(r => r.RoomCode == answerDto.RoomCode && r.Status == GameStatus.InProgress);

                if (room == null)
                {
                    _logger.LogWarning($"SubmitAnswer: Soba {answerDto.RoomCode} nije pronađena ili igra nije aktivna.");
                    return;
                }

                var participant = room.Participants.FirstOrDefault(p => p.UserId.ToString() == userIdStr);
                if (participant == null)
                {
                    _logger.LogWarning($"SubmitAnswer: Korisnik {userIdStr} nije učesnik u sobi {answerDto.RoomCode}.");
                    return;
                }

                var hasAlreadyAnswered = await context.ParticipantAnswers
                    .AnyAsync(pa => pa.ParticipantId == participant.Id && pa.QuestionId == answerDto.QuestionId);

                if (hasAlreadyAnswered)
                {
                    _logger.LogWarning($"SubmitAnswer: Učesnik {participant.Id} je već odgovorio na pitanje {answerDto.QuestionId}.");
                    return; 
                }

                var question = await context.Questions
                    .Include(q => q.AnswerOptions)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(q => q.QuestionID == answerDto.QuestionId);

                if (question == null || room.CurrentQuestionStartTime == null)
                {
                    _logger.LogWarning($"SubmitAnswer: Pitanje {answerDto.QuestionId} nije pronađeno ili vreme za početak nije postavljeno.");
                    return;
                }

                var timeTaken = DateTime.UtcNow - room.CurrentQuestionStartTime.Value;
                bool isCorrect = IsLiveAnswerCorrect(question, answerDto.SelectedOptionIds ?? new List<int>(), answerDto.TextAnswer);
                int pointsAwarded = 0;
                if (isCorrect)
                {
                    pointsAwarded = (int)question.PointNum;
                    double timeLimit = question.TimeLimitSeconds ?? 15.0;
                    if (timeLimit > 0)
                    {
                        double speedBonusRatio = 1.0 - (timeTaken.TotalSeconds / timeLimit);
                        if (speedBonusRatio < 0) speedBonusRatio = 0;
                        int bonusPoints = (int)Math.Round((pointsAwarded / 2.0) * speedBonusRatio);
                        pointsAwarded += bonusPoints;
                    }
                }

                var participantAnswer = new ParticipantAnswer
                {
                    ParticipantId = participant.Id,
                    QuestionId = answerDto.QuestionId,
                    SubmittedAnswer = answerDto.TextAnswer ?? string.Join(",", answerDto.SelectedOptionIds),
                    AnswerTimeMilliseconds = (int)timeTaken.TotalMilliseconds,
                    IsCorrect = isCorrect,
                    PointsAwarded = pointsAwarded,
                };
                context.ParticipantAnswers.Add(participantAnswer);
                participant.Score += pointsAwarded;

                context.Entry(participant).State = EntityState.Modified;
                await context.SaveChangesAsync();

                if (answerDto.SelectedOptionIds != null && answerDto.SelectedOptionIds.Any())
                {
                    var selectedOptions = answerDto.SelectedOptionIds.Select(id => new ParticipantSelectedOption
                    {
                        ParticipantAnswerId = participantAnswer.Id,
                        AnswerOptionId = id
                    }).ToList();
                    await context.ParticipantSelectedOptions.AddRangeAsync(selectedOptions);
                    await context.SaveChangesAsync();
                }

                _logger.LogInformation($"[LiveGameManager] Odgovor sačuvan za učesnika {participant.Id}. Novi skor: {participant.Score}.");

                var updatedRoomForDto = await context.GameRooms.Include(r => r.Participants).ThenInclude(p => p.User).FirstAsync(r => r.Id == room.Id);
                await _hubContext.Clients.Group(answerDto.RoomCode).SendAsync("UpdateRoom", MapRoomToDto(updatedRoomForDto));

                var totalParticipants = room.Participants.Count(p => p.User.Username != room.HostUsername);
                var answersForThisQuestion = await context.ParticipantAnswers.CountAsync(pa => pa.QuestionId == answerDto.QuestionId && room.Participants.Select(p => p.Id).Contains(pa.ParticipantId));

                if (totalParticipants > 0 && answersForThisQuestion >= totalParticipants)
                {
                    _logger.LogInformation($"[LiveGameManager] Svi su odgovorili u sobi {room.RoomCode}. Prelazim na sledeće pitanje.");
                    CancelQuestionTimer(room.RoomCode);
                    await AdvanceToNextQuestion(room.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FATALNA GREŠKA U SubmitAnswerAsync metodi!");
                await _hubContext.Clients.Client(connectionId).SendAsync("Error", "Došlo je do greške prilikom slanja odgovora.");
            }
        }

        public async Task AdvanceAfterTimerAsync(string roomCode)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();
            var room = await context.GameRooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room != null) await AdvanceToNextQuestion(room.Id);
        }
        #endregion

        #region Private Helpers

        private async Task HandleDisconnect(string username)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var room = await context.GameRooms
                .Include(r => r.Participants).ThenInclude(p => p.User)
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.HostUsername == username || r.Participants.Any(p => p.User.Username == username));

            if (room == null || room.Status == GameStatus.Finished) return;

            if (room.HostUsername == username)
            {
                _logger.LogInformation($"[LiveGameManager] Domaćin '{username}' se diskonektovao iz sobe '{room.RoomCode}'.");
                room.HostDisconnectedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
                await _hubContext.Clients.Group(room.RoomCode).SendAsync("HostDisconnected", "Domaćin se privremeno diskonektovao. Igra će biti prekinuta za 60 sekundi ako se ne vrati.");

                _ = TerminateRoomAfterDelay(room.Id);

            }
            else
            {
                var participant = room.Participants.FirstOrDefault(p => p.User.Username == username);
                if (participant != null)
                {
                    _logger.LogInformation($"[LiveGameManager] Igrač '{username}' se diskonektovao. Označavam ga kao neaktivnog.");
                    participant.DisconnectedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    var updatedRoom = await context.GameRooms
                        .Include(r => r.Participants).ThenInclude(p => p.User)
                        .AsNoTracking()
                        .FirstAsync(r => r.Id == room.Id);

                    var roomDto = MapRoomToDto(updatedRoom);
                    await _hubContext.Clients.Group(room.RoomCode).SendAsync("UpdateRoom", roomDto);
                }
            }
        }

        private async Task SendQuestion(int roomId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var room = await context.GameRooms
                .Include(r => r.Quiz.Questions).ThenInclude(q => q.AnswerOptions)
                .AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null || room.CurrentQuestionIndex >= room.Quiz.Questions.Count)
            {
                await AdvanceToNextQuestion(roomId);
                return;
            }

            // Moramo da ažuriramo vreme početka pitanja u bazi
            var roomInDb = await context.GameRooms.FindAsync(roomId);
            if (roomInDb != null)
            {
                roomInDb.CurrentQuestionStartTime = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }

            var question = room.Quiz.Questions.OrderBy(q => q.QuestionID).ToList()[room.CurrentQuestionIndex];
            var questionDto = MapQuestionToDto(question, room);
            await _hubContext.Clients.Group(room.RoomCode).SendAsync("NewQuestion", questionDto);

            StartQuestionTimer(room.RoomCode, question.TimeLimitSeconds ?? 15);
        }

        private async Task AdvanceToNextQuestion(int roomId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var roomForCode = await context.GameRooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomId);
            if (roomForCode == null) return;

            if (!_advancingRooms.TryAdd(roomForCode.RoomCode, true))
            {
                _logger.LogWarning($"[LiveGameManager] AdvanceToNextQuestion je već u toku za sobu {roomForCode.RoomCode}. Izlazim.");
                return;
            }

            try
            {
                var room = await context.GameRooms
                    .Include(r => r.Quiz.Questions)
                    .Include(r => r.Participants).ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(r => r.Id == roomId);

                if (room == null || room.Status != GameStatus.InProgress) return;

                if (room.CurrentQuestionIndex >= room.Quiz.Questions.Count - 1)
                {
                    room.Status = GameStatus.Finished;
                    room.FinishedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();

                    var finalRoomDto = MapRoomToDto(room);
                    await _hubContext.Clients.Group(room.RoomCode).SendAsync("GameFinished", finalRoomDto);
                    _logger.LogInformation($"[LiveGameManager] Igra u sobi {room.RoomCode} je završena.");
                }
                else
                {
                    room.CurrentQuestionIndex++;
                    await context.SaveChangesAsync();
                    await SendQuestion(roomId);
                    _logger.LogInformation($"[LiveGameManager] Soba {room.RoomCode} prelazi na pitanje {room.CurrentQuestionIndex}.");
                }
            }
            finally
            {
                _advancingRooms.TryRemove(roomForCode.RoomCode, out _);
            }
        }

        private void StartQuestionTimer(string roomCode, int timeLimitSeconds)
        {
            CancelQuestionTimer(roomCode); // Uvek otkaži prethodni tajmer
            var cts = new CancellationTokenSource();
            _roomTimers[roomCode] = cts;

            // Vreme za čitanje pitanja (5s) + vreme za odgovor
            int delay = (timeLimitSeconds * 1000) + 5000;

            Task.Delay(delay, cts.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    _logger.LogInformation($"[LiveGameManager] Tajmer za sobu {roomCode} je uspešno otkazan.");
                    return;
                }

                _logger.LogInformation($"[LiveGameManager] Tajmer za sobu {roomCode} je istekao. Pokrećem AdvanceToNextQuestion.");
                // Kreiramo novi scope jer ovaj kod radi u pozadini
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();
                var room = context.GameRooms.AsNoTracking().FirstOrDefault(r => r.RoomCode == roomCode);
                if (room != null)
                {
                    // Direktno pozivamo metodu, jer smo unutar servisa
                    AdvanceToNextQuestion(room.Id).Wait();
                }
            });
        }

        private void CancelQuestionTimer(string roomCode)
        {
            if (_roomTimers.TryRemove(roomCode, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }

        public GameRoomDto MapRoomToDto(GameRoom room)
        {
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

        private LiveQuestionDto MapQuestionToDto(Models.Question question, GameRoom room)
        {
            return new LiveQuestionDto
            {
                QuestionID = question.QuestionID,
                QuestionText = question.QuestionText,
                Type = question.Type.ToString(),
                TimeLimitSeconds = question.TimeLimitSeconds,
                PointNum = question.PointNum,
                AnswerOptions = question.AnswerOptions.Select(o => new AnswerOptionDto { AnswerOptionID = o.AnswerOptionID, Text = o.Text }).ToList(),
                CurrentQuestionIndex = room.CurrentQuestionIndex,
                TotalQuestions = room.Quiz.Questions.Count
            };
        }

        private bool IsLiveAnswerCorrect(Models.Question question, List<int> selectedOptionIds, string textAnswer)
        {
            if (question.Type == QuestionType.FillInTheBlank)
            {
                return textAnswer?.Trim().Equals(question.CorrectTextAnswer?.Trim(), StringComparison.OrdinalIgnoreCase) ?? false;
            }
            else
            {
                var correctOptionIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.AnswerOptionID).ToHashSet();
                var selectedIdsHashSet = selectedOptionIds.ToHashSet();
                return correctOptionIds.SetEquals(selectedIdsHashSet);
            }
        }

        private async Task CheckAndTerminateRoom(int roomId)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KvizHubContext>();

            var room = await context.GameRooms.FindAsync(roomId);

            if (room != null && room.HostDisconnectedAt.HasValue && room.Status != GameStatus.Finished)
            {
                _logger.LogInformation($"[LiveGameManager] Domaćin se nije vratio na vreme u sobu {room.RoomCode}. Gasim sobu.");
                room.Status = GameStatus.Finished;
                await context.SaveChangesAsync();

                await _hubContext.Clients.Group(room.RoomCode).SendAsync("GameTerminated", "Domaćin je napustio igru. Kviz je završen.");
            }
        }

        private async Task TerminateRoomAfterDelay(int roomId)
        {
            try
            {
                // Čekamo 60 sekundi
                await Task.Delay(TimeSpan.FromSeconds(60));

                // Nakon čekanja, pozivamo postojeću logiku za proveru i gašenje sobe
                await CheckAndTerminateRoom(roomId);
            }
            catch (Exception ex)
            {
                // Hvatamo BILO KOJU grešku koja se desi u pozadini i logujemo je
                _logger.LogError(ex, $"Greška prilikom automatskog gašenja sobe sa ID-jem {roomId} nakon diskonekcije domaćina.");
            }
        }

        #endregion

    }
}