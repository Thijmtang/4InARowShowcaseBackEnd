namespace DotNetAuth.Models.DTO
{
    public class GameLobbyDTO
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }

        public string CurrentPlayerTurn { get; set; }

        public List<GameCell> GameField { get; set; }


    }
}
