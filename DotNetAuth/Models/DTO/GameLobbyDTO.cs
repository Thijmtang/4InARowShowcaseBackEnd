using Microsoft.IdentityModel.Tokens;

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

        public Dictionary<string, GamePlayerDto> Players { get;  set; } = new();

        public string CurrentPlayerTurn { get; set; } = "";

        public List<GameCell> GameField { get; set; } = new();
        public string Winner { get; set; }
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

            var deletedPlayerType = Players[connectionId].PlayerType;
            Players.Remove(connectionId);


            // If player is getting removed, 1 move down
            if (deletedPlayerType == PlayerType.Player1)
            {
                var player2 = Players.First().Value;
                player2.PlayerType = PlayerType.Player1;

                Players[player2.ConnectionId] = player2;

            }
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

            PlayerType type = PlayerType.Player1;

            if (Players.Count == 1)
            {
                type = PlayerType.Player2;
            }

            player.PlayerType = type;
            
            Players.Add(player.ConnectionId, player);
        }

        public void StartGame()
        {
            if (Players.Count != 2)
            {
                throw new Exception("Niet genoeg spelers in lobby");
            }

            Status = STATUS.ONGOING;
            GameField = GenerateField();

            CurrentPlayerTurn = GetPlayer(PlayerType.Player1).ConnectionId;
        }

        public void ClickCell(int x)
        {
            // Group cells by X coordinate for faster lookup
            var cellsByX = GameField.GroupBy(c => c.X).ToDictionary(g => g.Key, g => g.ToList());

            // Track cells with value 0 for faster filtering
            var zeroValueCells = new HashSet<GameCell>(GameField.Where(c => c.Value == 0));

            // Iterate through cells and perform operations
            foreach (var cell in GameField)
            {
                cell.New = false;
            }

            if (cellsByX.TryGetValue(x, out var cellsWithX))
            {

                var newCell = cellsWithX.Where(c => zeroValueCells.Contains(c)).OrderByDescending(c => c.Y).FirstOrDefault();
                if (newCell != null)
                {
                    newCell.Value = (int)GetPlayers()[CurrentPlayerTurn].PlayerType;
                    newCell.New = true;
                }

                if (CheckWin(newCell))
                {
                    this.Winner = CurrentPlayerTurn;
                    this.Status = STATUS.COMPLETED;
                    Winner = CurrentPlayerTurn;
                }

                // Toggle turn, reverse search the other player
                CurrentPlayerTurn = GetPlayers().Values.First(p => p.ConnectionId != CurrentPlayerTurn).ConnectionId;
            }

        }


        public GamePlayerDto GetPlayer(PlayerType type)
        {
            var player = GetPlayers().Values.First(p => p.PlayerType == type);
            return player;
        }

        private List<GameCell> GenerateField(int width = 8, int height = 8)
        {
            var gameField = new List<GameCell>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    gameField.Add(new GameCell()
                    {
                        X = x,
                        Y = y,
                    });

                }
            }

            return gameField;
        }

        public Dictionary<string, GamePlayerDto> GetPlayers()
        {
            return this.Players;
        }

        public bool CheckWin(GameCell placedCell)
        {
            var horizontalRow = GameField.FindAll(gc => gc.Y == placedCell.Y);
            var VerticalRow = GameField.FindAll(gc => gc.X == placedCell.X);

            if (CheckSequence(horizontalRow, placedCell) || CheckSequence(VerticalRow, placedCell))
            {
                return true;
            }

            var count = 0;

            // Bottom left to top right diagonal
            for (int offset = -3; offset <= 3; offset++) // Check the diagonal starting from four positions below to four positions above the placed cell
            {
                int x = placedCell.X + offset;
                int y = placedCell.Y + offset;

                if (!IsInsideBoard(x, y) || !GameField.Exists(gc => gc.X == x && gc.Y == y && gc.Value == placedCell.Value))
                {
                    count = 0;
                    continue;
                }

                count++;

                if (count == 4)
                {
                    return true;
                }
            }

            count = 0;

            // Top left to bottom right diagonal
            for (int offset = -3; offset <= 3; offset++) // Check the diagonal starting from four positions above to four positions below the placed cell
            {
                int x = placedCell.X + offset;
                int y = placedCell.Y - offset;

                if (!IsInsideBoard(x, y) || !GameField.Exists(gc => gc.X == x && gc.Y == y && gc.Value == placedCell.Value))
                {
                    count = 0;
                    continue;
                }

                count++;
                if (count == 4)
                {
                    return true;
                }
            }



            return false;
        }

        private bool CheckSequence(List<GameCell> sequence, GameCell placedCell)
        {
            int count = 0;
            foreach (var cell in sequence)
            {
                if (cell.Value != placedCell.Value)
                {
                    count = 0;
                    continue;
                }
                count++;
                if (count == 4)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }
}
