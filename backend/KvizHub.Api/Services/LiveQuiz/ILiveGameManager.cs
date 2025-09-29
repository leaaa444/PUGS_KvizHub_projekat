using KvizHub.Api.Dtos.GameRoom;

namespace KvizHub.Api.Services.LiveQuiz
{
    public interface ILiveGameManager
    {
        void UserConnected(string connectionId, string username);
        Task UserDisconnectedAsync(string connectionId);

        Task CreateRoomAsync(int quizId, string hostUsername, string connectionId);
        Task<(GameRoomDto? room, bool isNewParticipant, bool gameInProgress)> EnterLobbyAsync(string roomCode, string username, int userId, string connectionId);
        Task StartGameAsync(string roomCode, string hostUsername);
        Task SubmitAnswerAsync(SubmitAnswerDto answerDto, string userIdStr, string connectionId);
        Task AdvanceAfterTimerAsync(string roomCode);

    }
}
