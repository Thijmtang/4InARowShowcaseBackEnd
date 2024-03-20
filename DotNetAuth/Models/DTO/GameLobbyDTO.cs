namespace DotNetAuth.Models.DTO
{
    public enum STATUS {
        STANDBY,
        ONGOING,
        COMPLETED,
    }

    public class GameLobbyDTO
    {
        public string Code { get; set; }
        public STATUS Status { get; set; } = new();

        private Dictionary<string, GamePlayerDto> Players { get;  set; } = new();

        public GamePlayerDto CurrentPlayerTurn { get; set; } = new();

        public List<GameCell> GameField { get; set; } = new();



        public GameLobbyDTO()
        {
            this.Status = STATUS.STANDBY;
        }

        public void RemovePlayer(string connectionId)
        {
            if (!Players.ContainsKey(connectionId))
            {
                throw new ArgumentException("Speler bestaat niet");
            }


            Players.Remove(connectionId);

        }

        public void AddPlayer(GamePlayerDto player)
        {
            if (Players.ContainsKey(player.ConnectionId))
            {
                throw new ArgumentException("Speler zit al in de lobby!");
            }

            if (Players.Count >= 2)
            {
                throw new Exception("Er mogen maximaal 2 spelers in een lobby zitten");
            }


            Players.Add(player.ConnectionId, player);
        }

        public Dictionary<string, GamePlayerDto> GetPlayers()
        {
            return this.Players;
        }


    }
}
