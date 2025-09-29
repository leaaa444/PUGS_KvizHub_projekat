namespace KvizHub.Api.Models.Enums
{
    public enum GameStatus
    {
        Lobby,       // Soba je otvorena, čeka igrače
        InProgress,  // Kviz je u toku
        Finished     // Kviz je završen, prikazuju se rezultati
    }
}
