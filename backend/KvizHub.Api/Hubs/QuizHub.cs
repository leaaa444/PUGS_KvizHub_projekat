//using KvizHub.Api.Data;
//using KvizHub.Api.Dtos.GameRoom;
//using KvizHub.Api.Dtos.Player;
//using KvizHub.Api.Models;
//using KvizHub.Api.Models.Enums;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Concurrent;
//using System.Security.Claims;

//namespace KvizHub.Api.Hubs
//{
//    [Authorize]
//    public class QuizHub : Hub
//    {
//        private readonly KvizHubContext _context;
//        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
//        private static readonly ConcurrentDictionary<string, bool> _advancingRooms = new(); 
//        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _roomTimers = new();
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly IHubContext<QuizHub> _hubContext;
//        private readonly ILogger<QuizHub> _logger;


//        public QuizHub(KvizHubContext context,
//                           ILogger<QuizHub> logger,
//                           IServiceScopeFactory scopeFactory,
//                           IHubContext<QuizHub> hubContext)
//        {
//            _context = context;
//            _logger = logger;
//            _scopeFactory = scopeFactory;
//            _hubContext = hubContext;
//        }

//        #region Connection Management
//        public override async Task OnConnectedAsync()
//        {
//            var username = Context.User?.FindFirstValue(ClaimTypes.Name);
//            _logger.LogInformation($"[OnConnectedAsync] Klijent se povezao. Korisnik: {username ?? "N/A"}, ConnectionId: {Context.ConnectionId}");
//            if (!string.IsNullOrEmpty(username))
//            {
//                _userConnections[Context.ConnectionId] = username;
//            }
//            await base.OnConnectedAsync();
//        }

//        public override async Task OnDisconnectedAsync(Exception? exception)
//        {
//            if (_userConnections.TryRemove(Context.ConnectionId, out var username))
//            {
//                _logger.LogInformation($"[OnDisconnectedAsync] Korisnik '{username}' se diskonektovao. ConnectionId: {Context.ConnectionId}");
//                await HandleDisconnect(username);
//            }
//            await base.OnDisconnectedAsync(exception);
//        }
//        #endregion

//        //#region Lobby Management
//        ////public async Task CreateRoom(int quizId)
//        ////{
//        ////    var username = Context.User?.FindFirstValue(ClaimTypes.Name);
//        ////    _logger.LogInformation($"[CreateRoom] Korisnik '{username}' pokreće kreiranje sobe za QuizId: {quizId}.");
//        ////    if (string.IsNullOrEmpty(username))
//        ////    {
//        ////        _logger.LogError("[CreateRoom] Neuspeh: Nije moguće pročitati username iz tokena.");
//        ////        await Clients.Caller.SendAsync("Error", "Nije moguće identifikovati korisnika.");
//        ////        return;
//        ////    }

//        ////    var roomCode = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
//        ////    var newRoomDb = new Models.GameRoom { RoomCode = roomCode, QuizId = quizId, HostUsername = username, Status = GameStatus.Lobby };

//        ////    _context.GameRooms.Add(newRoomDb);
//        ////    await _context.SaveChangesAsync();
//        ////    _logger.LogInformation($"[CreateRoom] Soba '{roomCode}' uspešno kreirana u bazi.");

//        ////    await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
//        ////    await Clients.Caller.SendAsync("RoomCreated", roomCode);
//        ////}

//        //public async Task EnterLobby(string roomCode)
//        //{
//        //    var username = Context.User?.FindFirstValue(ClaimTypes.Name);
//        //    var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
//        //    _logger.LogInformation($"[EnterLobby] Korisnik '{username}' pokušava da uđe u sobu '{roomCode}'.");

//        //    var room = await _context.GameRooms
//        //        .Include(r => r.Quiz.Questions).ThenInclude(q => q.AnswerOptions)
//        //        .Include(r => r.Participants).ThenInclude(p => p.User)
//        //        .FirstOrDefaultAsync(r => r.RoomCode == roomCode);
//        //    if (room == null)
//        //    {
//        //        _logger.LogWarning($"[EnterLobby] Neuspeh: Soba '{roomCode}' ne postoji.");
//        //        await Clients.Caller.SendAsync("Error", "Soba ne postoji.");
//        //        return;
//        //    }
//        //    if (room.Status == GameStatus.Finished)
//        //    {
//        //        _logger.LogWarning($"[EnterLobby] Neuspeh: Igra u sobi '{roomCode}' je završena.");
//        //        await Clients.Caller.SendAsync("Error", "Igra u ovoj sobi je završena.");
//        //        return;
//        //    }

//        //    if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(username))
//        //    {
//        //        _logger.LogError($"[EnterLobby] Neuspeh: Token za korisnika koji ulazi u sobu '{roomCode}' je nevalidan.");
//        //        return;
//        //    }

//        //    var userId = int.Parse(userIdStr);
//        //    await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

//        //    var participant = room.Participants.FirstOrDefault(p => p.UserId == userId);
//        //    bool isNewParticipant = false;
//        //    if (participant == null && room.HostUsername != username)
//        //    {
//        //        _logger.LogInformation($"[EnterLobby] Korisnik '{username}' je igrač, dodajem ga kao učesnika u sobu '{roomCode}'.");
//        //        room.Participants.Add(new Models.LiveQuizParticipant { UserId = userId, Score = 0 });
//        //        await _context.SaveChangesAsync();
//        //        isNewParticipant = true;
//        //    }
//        //    else
//        //    {
//        //        _logger.LogInformation($"[EnterLobby] Korisnik '{username}' je host ili je već u sobi '{roomCode}'. Ne dodajem ga ponovo.");
//        //    }
//        //    if (isNewParticipant)
//        //    {
//        //        // 1. Ako je ušao NOVI igrač, ponovo učitaj sobu da dobiješ SVEŽE podatke
//        //        var updatedRoom = await _context.GameRooms
//        //            .Include(r => r.Participants).ThenInclude(p => p.User)
//        //            .AsNoTracking() // Dobra praksa jer samo čitamo podatke
//        //            .FirstOrDefaultAsync(r => r.RoomCode == roomCode);

//        //        if (updatedRoom != null)
//        //        {
//        //            // 2. Mapiraj SVEŽE podatke i pošalji SVIMA u grupi (uključujući i novog igrača)
//        //            var roomDtoForAll = MapRoomToDto(updatedRoom);
//        //            _logger.LogInformation($"[BACKEND->SVI] Novi igrač '{username}' je ušao. Šaljem ažuriranu sobu svima.");
//        //            await Clients.Group(roomCode).SendAsync("UpdateRoom", roomDtoForAll);
//        //        }
//        //    }
//        //    else
//        //    {
//        //        // 3. Ako je igrač već bio u sobi (npr. reconnect), pošalji stanje samo njemu
//        //        var roomDtoForCaller = MapRoomToDto(room); // Koristimo 'room' sa početka jer nema novih igrača
//        //        _logger.LogInformation($"[BACKEND->POZIVAOCU] Igrač '{username}' se ponovo povezao. Šaljem mu trenutno stanje.");
//        //        await Clients.Caller.SendAsync("UpdateRoom", roomDtoForCaller);
//        //    }



//        //    // 3. Ako je igra već u toku, pošalji POZIVAOCU i trenutno pitanje
//        //    if (room.Status == GameStatus.InProgress)
//        //    {
//        //        _logger.LogInformation($"[EnterLobby] Igra u sobi '{roomCode}' je u toku. Šaljem trenutno pitanje pozivaocu.");
//        //        await Clients.Caller.SendAsync("GameStarted");
//        //    }

//        //    _logger.LogInformation($"[EnterLobby] KRAJ: Završen proces za korisnika '{username}'.");

//        //}
//        //#endregion

//        #region Game Flow
//        public async Task StartGame(string roomCode)
//        {
//            var username = Context.User?.FindFirstValue(ClaimTypes.Name);
//            _logger.LogInformation($"[StartGame] Korisnik '{username}' pokušava da pokrene igru u sobi '{roomCode}'.");
//            var room = await _context.GameRooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);

//            if (room == null || room.HostUsername != username)
//            {
//                _logger.LogWarning($"[StartGame] Neuspeh: Korisnik '{username}' nije host sobe '{roomCode}' ili soba ne postoji.");
//                return;
//            }

//            room.Status = GameStatus.InProgress;
//            await _context.SaveChangesAsync();
//            await Clients.Group(roomCode).SendAsync("GameStarted");
//            _logger.LogInformation($"[StartGame] Igra u sobi '{roomCode}' je uspešno pokrenuta. Šaljem prvo pitanje...");

//            await SendQuestion(room, _context, _hubContext, _scopeFactory, _logger);
//        }

//        public async Task SubmitAnswer(SubmitAnswerDto answerDto)
//        {
//            _logger.LogInformation($"[SubmitAnswer] POZVAN. Soba: {answerDto.RoomCode}, PitanjeId: {answerDto.QuestionId}");

//            try
//            {
//                var roomCode = answerDto.RoomCode;
//                var questionId = answerDto.QuestionId;
//                var selectedOptionIds = answerDto.SelectedOptionIds ?? new List<int>();
//                var textAnswer = answerDto.TextAnswer;
//                var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

//                if (string.IsNullOrEmpty(userIdStr))
//                    if (string.IsNullOrEmpty(userIdStr)) { _logger.LogWarning("[SubmitAnswer] Neuspeh: UserId nije u tokenu."); return; }

//                var room = await _context.GameRooms
//                    .Include(r => r.Participants)
//                    .FirstOrDefaultAsync(r => r.RoomCode == roomCode && r.Status == GameStatus.InProgress);

//                if (room == null)
//                {
//                    _logger.LogWarning($"SubmitAnswer: Soba {roomCode} nije pronađena ili igra nije aktivna.");
//                    return;
//                }

//                var participant = room.Participants.FirstOrDefault(p => p.UserId.ToString() == userIdStr);
//                if (participant == null)
//                {
//                    _logger.LogWarning($"SubmitAnswer: Korisnik {userIdStr} nije učesnik u sobi {roomCode}.");
//                    return;
//                }

//                var hasAlreadyAnswered = await _context.ParticipantAnswers
//                    .AnyAsync(pa => pa.ParticipantId == participant.Id && pa.QuestionId == questionId);

//                if (hasAlreadyAnswered)
//                {
//                    _logger.LogWarning($"SubmitAnswer: Učesnik {participant.Id} je već odgovorio na pitanje {questionId}.");
//                    return;
//                }

//                var question = await _context.Questions
//                    .Include(q => q.AnswerOptions)
//                    .AsNoTracking() 
//                    .FirstOrDefaultAsync(q => q.QuestionID == questionId);

//                if (question == null || room.CurrentQuestionStartTime == null)
//                {
//                    _logger.LogWarning($"SubmitAnswer: Pitanje {questionId} nije pronađeno ili vreme za početak nije postavljeno.");
//                    return;
//                }

//                // --- Računanje poena ---
//                var timeTaken = DateTime.UtcNow - room.CurrentQuestionStartTime.Value;
//                bool isCorrect = IsLiveAnswerCorrect(question, selectedOptionIds, textAnswer);
//                int pointsAwarded = 0;
//                if (isCorrect)
//                {
//                    pointsAwarded = (int)question.PointNum;
//                    double timeLimit = question.TimeLimitSeconds ?? 15.0;
//                    if (timeLimit > 0)
//                    {
//                        double speedBonusRatio = 1.0 - (timeTaken.TotalSeconds / timeLimit);
//                        if (speedBonusRatio < 0) speedBonusRatio = 0;
//                        int bonusPoints = (int)Math.Round((pointsAwarded / 2.0) * speedBonusRatio);
//                        pointsAwarded += bonusPoints;
//                    }
//                }
//                _logger.LogInformation($"[SubmitAnswer] Računam poene za učesnika {participant.Id}.");

//                var participantAnswer = new ParticipantAnswer
//                {
//                    ParticipantId = participant.Id,
//                    QuestionId = questionId,
//                    SubmittedAnswer = textAnswer ?? string.Join(",", selectedOptionIds),
//                    AnswerTimeMilliseconds = (int)timeTaken.TotalMilliseconds,
//                    IsCorrect = isCorrect,
//                    PointsAwarded = pointsAwarded,
//                };
//                _context.ParticipantAnswers.Add(participantAnswer);
//                participant.Score += pointsAwarded;
//                await _context.SaveChangesAsync();

//                if (selectedOptionIds.Any())
//                {
//                    var selectedOptions = selectedOptionIds.Select(id => new ParticipantSelectedOption
//                    {
//                        ParticipantAnswerId = participantAnswer.Id, 
//                        AnswerOptionId = id
//                    }).ToList();

//                    await _context.ParticipantSelectedOptions.AddRangeAsync(selectedOptions);
//                    await _context.SaveChangesAsync(); 
//                }


//                _logger.LogInformation($"[SubmitAnswer] Odgovor sačuvan za učesnika {participant.Id}.");


//                var updatedRoomForDto = await _context.GameRooms.Include(r => r.Participants).ThenInclude(p => p.User).FirstAsync(r => r.Id == room.Id);
//                await _hubContext.Clients.Group(roomCode).SendAsync("UpdateRoom", MapRoomToDto(updatedRoomForDto));

//                var totalParticipants = await _context.LiveQuizParticipants.CountAsync(p => p.GameRoomId == room.Id);
//                var participantIds = room.Participants.Select(p => p.Id).ToList();
//                var answersForThisQuestion = await _context.ParticipantAnswers
//                    .CountAsync(pa => pa.QuestionId == questionId && participantIds.Contains(pa.ParticipantId));

//                _logger.LogInformation($"[SubmitAnswer] Provera za sledeće pitanje u sobi {roomCode}: Odgovora {answersForThisQuestion}/{totalParticipants}.");

//                if (totalParticipants > 0 && answersForThisQuestion >= totalParticipants)
//                {
//                    _logger.LogInformation($"[SubmitAnswer] Svi su odgovorili! Poništavam tajmer i pokrećem AdvanceToNextQuestion za sobu {room.RoomCode}.");

//                    if (_roomTimers.TryRemove(room.RoomCode, out var cts))
//                    {
//                        cts.Cancel();
//                        cts.Dispose();
//                    }

//                    _ = Task.Run(() => {
//                        using (var scope = _scopeFactory.CreateScope())
//                        {
//                            var scopedDbContext = scope.ServiceProvider.GetRequiredService<KvizHubContext>();
//                            var scopedHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<QuizHub>>();
//                            var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<QuizHub>>();
//                            AdvanceToNextQuestion(room.RoomCode, room.Id, scopedDbContext, scopedHubContext, _scopeFactory, scopedLogger).Wait();
//                        }
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "FATALNA GREŠKA U SubmitAnswer metodi!");
//                await Clients.Caller.SendAsync("Error", "Došlo je do neočekivane greške na serveru prilikom slanja odgovora.");
//            }
//        }
//        #endregion

//        #region Helper Methods

//        //private async Task HandleDisconnect(string username)
//        //{
//        //    var room = await _context.GameRooms
//        //                             .Include(r => r.Participants).ThenInclude(p => p.User)
//        //                             .FirstOrDefaultAsync(r => r.HostUsername == username || r.Participants.Any(p => p.User.Username == username));
//        //    if (room == null) return;

//        //    if (room.HostUsername == username)
//        //    {
//        //        room.HostDisconnectedAt = DateTime.UtcNow;
//        //        await _context.SaveChangesAsync();
//        //        await Clients.Group(room.RoomCode).SendAsync("HostDisconnected", "Domaćin se privremeno diskonektovao.");
//        //    }
//        //    else
//        //    {
//        //        var participant = room.Participants.FirstOrDefault(p => p.User.Username == username);
//        //        if (participant != null)
//        //        {
//        //            _context.LiveQuizParticipants.Remove(participant);
//        //            await _context.SaveChangesAsync();

//        //            var roomDto = MapRoomToDto(room);
//        //            await Clients.Group(room.RoomCode).SendAsync("UpdateRoom", roomDto);
//        //        }
//        //    }
//        //}

//        //private static GameRoomDto MapRoomToDto(Models.GameRoom room)
//        //{
//        //    return new GameRoomDto
//        //    {
//        //        RoomCode = room.RoomCode,
//        //        QuizId = room.QuizId,
//        //        HostUsername = room.HostUsername ?? "N/A", // Osiguravamo da HostUsername nikad nije null
//        //        Status = room.Status.ToString(),
//        //        CurrentQuestionIndex = room.CurrentQuestionIndex,
//        //        CurrentQuestionStartTime = room.CurrentQuestionStartTime,
//        //        Players = room.Participants
//        //            .Where(p => p.User != null) // Filtriramo za svaki slučaj
//        //            .Select(p => new PlayerDto
//        //            {
//        //                Username = p.User!.Username ?? "Nepoznat igrač", // ! operator kaže kompajleru da znamo da User nije null ovde
//        //                Score = p.Score,
//        //                ImageUrl = p.User!.ProfilePictureUrl
//        //            }).ToList()
//        //    };
//        //}

//        private static async Task SendQuestion(Models.GameRoom room, KvizHubContext dbContext, IHubContext<QuizHub> hubContext, IServiceScopeFactory scopeFactory, ILogger logger)
//        {
//            logger.LogInformation($"[SendQuestion] Pozvano za sobu {room.RoomCode}, indeks pitanja {room.CurrentQuestionIndex}.");

//            var quizWithQuestions = await dbContext.Quizzes
//                .Include(q => q.Questions).ThenInclude(q => q.AnswerOptions)
//                .AsNoTracking()
//                .FirstOrDefaultAsync(q => q.QuizID == room.QuizId);

//            if (quizWithQuestions == null || room.CurrentQuestionIndex >= quizWithQuestions.Questions.Count)
//            {
//                _ = Task.Run(() => {
//                    using var scope = scopeFactory.CreateScope();
//                    var scopedDbContext = scope.ServiceProvider.GetRequiredService<KvizHubContext>();
//                    var scopedHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<QuizHub>>();
//                    var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<QuizHub>>();
//                    AdvanceToNextQuestion(room.RoomCode, room.Id, scopedDbContext, scopedHubContext, scopeFactory, scopedLogger).Wait();
//                });
//                return;
//            }

//            var question = quizWithQuestions.Questions.OrderBy(q => q.QuestionID).ToList()[room.CurrentQuestionIndex];

//            var roomInDb = await dbContext.GameRooms.FindAsync(room.Id);
//            if (roomInDb != null)
//            {
//                roomInDb.CurrentQuestionStartTime = DateTime.UtcNow;
//                await dbContext.SaveChangesAsync();
//            }

//            var questionDto = new
//            {
//                QuestionId = question.QuestionID,
//                Type = question.Type.ToString(),
//                PointNum = question.PointNum,
//                QuestionText = question.QuestionText,
//                TimeLimit = question.TimeLimitSeconds,
//                AnswerOptions = question.AnswerOptions.Select(o => new { o.AnswerOptionID, o.Text }).ToList(),
//                CurrentQuestionIndex = room.CurrentQuestionIndex,
//                TotalQuestions = quizWithQuestions.Questions.Count,
//                CurrentQuestionStartTime = roomInDb.CurrentQuestionStartTime
//            };

//            await hubContext.Clients.Group(room.RoomCode).SendAsync("NewQuestion", questionDto);
//            if (_roomTimers.TryRemove(room.RoomCode, out var oldCts))
//            {
//                oldCts.Cancel();
//                oldCts.Dispose();
//            }

//            var cts = new CancellationTokenSource();
//            _roomTimers[room.RoomCode] = cts;

//            int delay = (question.TimeLimitSeconds ?? 15) * 1000 + 6000;

//            _ = Task.Run(async () =>
//            {
//                try
//                {
//                    await Task.Delay(delay, cts.Token);

//                    logger.LogInformation($"[SendQuestion Timer] Vreme za pitanje isteklo za sobu {room.RoomCode}. Pokrećem AdvanceToNextQuestion.");

//                    using (var scope = scopeFactory.CreateScope())
//                    {
//                        var provider = scope.ServiceProvider;
//                        var scopedDbContext = provider.GetRequiredService<KvizHubContext>();
//                        var scopedHubContext = provider.GetRequiredService<IHubContext<QuizHub>>();
//                        var scopedLogger = provider.GetRequiredService<ILogger<QuizHub>>();
//                        await AdvanceToNextQuestion(room.RoomCode, room.Id, scopedDbContext, scopedHubContext, scopeFactory, scopedLogger);
//                    }
//                }
//                catch (TaskCanceledException)
//                {
//                    logger.LogInformation($"[SendQuestion Timer] Tajmer za sobu {room.RoomCode} je uspešno poništen.");
//                }
//                catch (Exception ex)
//                {
//                    logger.LogError(ex, $"[SendQuestion Timer] Greška u pozadinskom tajmeru za sobu {room.RoomCode}.");
//                }
//                finally
//                {
//                    if (_roomTimers.TryRemove(room.RoomCode, out var finalCts))
//                    {
//                        finalCts.Dispose();
//                    }
//                }
//            });

//        }

//        private static async Task AdvanceToNextQuestion(string roomCode, int roomId, KvizHubContext dbContext, IHubContext<QuizHub> hubContext, IServiceScopeFactory scopeFactory, ILogger logger)
//        {
//            logger.LogInformation($"[AdvanceToNextQuestion] Metoda pozvana za sobu: {roomCode}. Pokušavam da zaključam...");

//            if (!_advancingRooms.TryAdd(roomCode, true))
//            {
//                logger.LogWarning($"[AdvanceToNextQuestion] Soba {roomCode} je već zaključana. Drugi proces već radi. Izlazim.");
//                return;
//            }

//            logger.LogInformation($"[AdvanceToNextQuestion] Soba {roomCode} uspešno zaključana.");

//            try
//            {
//                var room = await dbContext.GameRooms
//                                 .Include(r => r.Quiz.Questions)
//                                 .Include(r => r.Participants).ThenInclude(p => p.User)
//                                 .FirstOrDefaultAsync(r => r.Id == roomId);

//                if (room == null)
//                {
//                    logger.LogError($"[AdvanceToNextQuestion] KRITIČNA GREŠKA: Soba sa ID-jem {roomId} nije pronađena u bazi!");
//                    return;
//                }

//                if (room.Status != GameStatus.InProgress)
//                {
//                    logger.LogWarning($"[AdvanceToNextQuestion] Igra u sobi {roomCode} nije u toku (Status: {room.Status}). Ne radim ništa.");
//                    return;
//                }

//                int questionCount = room.Quiz?.Questions?.Count ?? 0;
//                int currentIndex = room.CurrentQuestionIndex;

//                logger.LogInformation($"[AdvanceToNextQuestion] Provera stanja za sobu {roomCode}: Trenutni indeks pitanja: {currentIndex}, Ukupno pitanja u kvizu: {questionCount}");

//                if (currentIndex >= questionCount - 1)
//                {
//                    logger.LogInformation($"[AdvanceToNextQuestion] Nema više pitanja ({currentIndex} >= {questionCount - 1}). Završavam igru za sobu {roomCode}.");

//                    room.Status = GameStatus.Finished;
//                    await dbContext.SaveChangesAsync();

//                    var roomDto = MapRoomToDto(room);

//                    await hubContext.Clients.Group(roomCode).SendAsync("GameFinished", roomDto);
//                }
//                else
//                {
//                    logger.LogInformation($"[AdvanceToNextQuestion] Ima još pitanja. Prelazim na sledeće pitanje za sobu {roomCode}.");

//                    room.CurrentQuestionIndex++;
//                    await dbContext.SaveChangesAsync();

//                    logger.LogInformation($"[AdvanceToNextQuestion] Indeks pitanja za sobu {roomCode} je sada {room.CurrentQuestionIndex}. Pozivam SendQuestion...");
//                    await SendQuestion(room, dbContext, hubContext, scopeFactory, logger);
//                }
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, $"[AdvanceToNextQuestion] Desila se greška u sobi {roomCode}.");
//            }
//            finally
//            {
//                logger.LogInformation($"[AdvanceToNextQuestion] Otključavam sobu {roomCode}.");
//                _advancingRooms.TryRemove(roomCode, out _);
//            }
//        }

//        private bool IsLiveAnswerCorrect(Models.Question question, List<int> selectedOptionIds, string textAnswer)
//        {
//            if (question.Type == QuestionType.FillInTheBlank)
//            {
//                return textAnswer?.Trim().Equals(question.CorrectTextAnswer?.Trim(), StringComparison.OrdinalIgnoreCase) ?? false;
//            }
//            else
//            {
//                var correctOptionIds = question.AnswerOptions.Where(o => o.IsCorrect).Select(o => o.AnswerOptionID).ToHashSet();
//                var selectedIdsHashSet = selectedOptionIds.ToHashSet();
//                return correctOptionIds.SetEquals(selectedIdsHashSet);
//            }
//        }

//        #endregion
//    }
//}

// Hubs/QuizHub.cs

using KvizHub.Api.Dtos.GameRoom;
using KvizHub.Api.Services;
using KvizHub.Api.Services.LiveQuiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace KvizHub.Api.Hubs;

[Authorize]
public class QuizHub : Hub
{
    private readonly ILiveGameManager _gameManager;

    public QuizHub(ILiveGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public override async Task OnConnectedAsync()
    {
        var username = GetUsername();
        if (!string.IsNullOrEmpty(username))
        {
            _gameManager.UserConnected(GetConnectionId(), username);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _gameManager.UserDisconnectedAsync(GetConnectionId());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(int quizId) => await _gameManager.CreateRoomAsync(quizId, GetUsername(), GetConnectionId());
    public async Task EnterLobby(string roomCode)
    {
        var username = GetUsername();
        var userIdStr = GetUserIdStr();
        var connectionId = GetConnectionId();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userIdStr)) return;

        var (room, isNew, inProgress) = await _gameManager.EnterLobbyAsync(roomCode, username, int.Parse(userIdStr), connectionId);

        if (room == null)
        {
            await Clients.Caller.SendAsync("Error", "Soba ne postoji ili je igra završena.");
            return;
        }

        await Groups.AddToGroupAsync(connectionId, roomCode);

        if (isNew)
        {
            await Clients.Group(roomCode).SendAsync("UpdateRoom", room);
        }
        else
        {
            await Clients.Caller.SendAsync("UpdateRoom", room);
        }

        if (inProgress)
        {
            await Clients.Caller.SendAsync("GameStarted");
        }
    }

    public async Task StartGame(string roomCode) => await _gameManager.StartGameAsync(roomCode, GetUsername());
    public async Task SubmitAnswer(SubmitAnswerDto dto) => await _gameManager.SubmitAnswerAsync(dto, GetUserIdStr(), GetConnectionId());

    public async Task OnTimerExpired(string roomCode) => await _gameManager.AdvanceAfterTimerAsync(roomCode);

    private string GetUsername() => Context.User?.FindFirstValue(ClaimTypes.Name);
    private string GetUserIdStr() => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    private string GetConnectionId() => Context.ConnectionId;
}