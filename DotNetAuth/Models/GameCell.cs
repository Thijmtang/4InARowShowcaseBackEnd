namespace DotNetAuth.Models
{
    public class GameCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; } = 0;
        public bool New { get; set; } = false;
    }
}
