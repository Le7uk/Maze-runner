using MazeRunner.Core.Models;

namespace MazeRunner.Core.Services;

public class MazeGenerator
{
    private readonly Random _random;

    public MazeGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public Cell[,] Generate(int size)
    {
        if (size % 2 == 0) size++;

        var grid = new Cell[size, size];

        for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                grid[row, col] = Cell.Wall;

        CarvePassages(grid, 1, 1, size);

        grid[1, 1]             = Cell.Start;
        grid[size - 2, size - 2] = Cell.Exit;

        return grid;
    }

    private void CarvePassages(Cell[,] grid, int row, int col, int size)
    {
        grid[row, col] = Cell.Floor;

        var directions = new (int dr, int dc)[] { (-2, 0), (2, 0), (0, -2), (0, 2) };
        Shuffle(directions);

        foreach (var (dr, dc) in directions)
        {
            int newRow = row + dr;
            int newCol = col + dc;

            if (newRow > 0 && newRow < size - 1 && newCol > 0 && newCol < size - 1
                && grid[newRow, newCol] == Cell.Wall)
            {
                grid[row + dr / 2, col + dc / 2] = Cell.Floor;
                CarvePassages(grid, newRow, newCol, size);
            }
        }
    }

    private void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public static bool AreAllFloorsReachable(Cell[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        (int startRow, int startCol) = (-1, -1);
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (grid[r, c] == Cell.Start)
                {
                    startRow = r;
                    startCol = c;
                }

        if (startRow == -1) return false;

        int totalFloors = 0;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (grid[r, c] != Cell.Wall)
                    totalFloors++;

        var visited = new bool[rows, cols];
        var queue   = new Queue<(int, int)>();
        queue.Enqueue((startRow, startCol));
        visited[startRow, startCol] = true;
        int reached = 0;

        int[] dRow = [-1, 1, 0, 0];
        int[] dCol = [0, 0, -1, 1];

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            reached++;

            for (int d = 0; d < 4; d++)
            {
                int nr = r + dRow[d];
                int nc = c + dCol[d];
                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols
                    && !visited[nr, nc] && grid[nr, nc] != Cell.Wall)
                {
                    visited[nr, nc] = true;
                    queue.Enqueue((nr, nc));
                }
            }
        }

        return reached == totalFloors;
    }
}
