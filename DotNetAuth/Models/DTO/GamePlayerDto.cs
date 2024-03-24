namespace DotNetAuth.Models.DTO
{
    public enum PlayerType
    {
        Player1 = 1,
        Player2 = 2,
    }

    public class GamePlayerDto
    {
        public string ConnectionId { get; set; }
        public string Username { get; set; }

        public DateTime JoinDate { get; set; }

        public PlayerType PlayerType { get; set; }
        
    }
}
