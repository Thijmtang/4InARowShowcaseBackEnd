using DotNetAuth.Models;

namespace DotNetAuth.Services
{
    public class GameService
    {
        public List<GameCell> GenerateField(int width = 8, int height = 8)
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
        public List<GameCell> clickCell(List<GameCell> field, GameCell clickedCell)
        {
            int x = clickedCell.X;

            // Group cells by X coordinate for faster lookup
            var cellsByX = field.GroupBy(c => c.X).ToDictionary(g => g.Key, g => g.ToList());

            // Track cells with value 0 for faster filtering
            var zeroValueCells = new HashSet<GameCell>(field.Where(c => c.Value == 0));

            // Iterate through cells and perform operations
            foreach (var cell in field)
            {
                cell.New = false;
            }

            if (cellsByX.TryGetValue(x, out var cellsWithX))
            {
                var newCell = cellsWithX.Where(c => zeroValueCells.Contains(c)).OrderByDescending(c => c.Y).FirstOrDefault();
                if (newCell != null)
                {
                    newCell.Value = 1;
                    newCell.New = true;
                }
            }

            return field;
        }

    }
}
