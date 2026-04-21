using MazeRunner.Core.Interfaces;
using MazeRunner.Core.Models;

namespace MazeRunner.Core.Services;

public class GameEngine : IGameEngine
{
    private readonly MazeGenerator _generator;
    private Random _random = new();

    public Level CurrentLevel       { get; private set; } = null!;
    public Player Player            { get; private set; } = null!;
    public bool IsCompleted         { get; private set; }
    public bool HasKey              { get; private set; }
    public bool ExitBlockedFeedback { get; private set; }

    public GameEngine(MazeGenerator? generator = null)
    {
        _generator = generator ?? new MazeGenerator();
    }

    public void LoadLevel(int levelNumber)
    {
        var settings = LevelConfig.GetLevel(levelNumber);

        var generator = new MazeGenerator(settings.Seed);
        var grid      = generator.Generate(settings.GridSize);

        _random = new Random(settings.Seed + 999);
        PlaceKey(grid, settings.GridSize);

        CurrentLevel        = new Level(levelNumber, grid);
        Player              = new Player(1, 1);
        IsCompleted         = false;
        HasKey              = false;
        ExitBlockedFeedback = false;
    }

    public void MovePlayer(Direction direction)
    {
        if (IsCompleted) return;

        ExitBlockedFeedback = false;

        int newRow = Player.Y;
        int newCol = Player.X;

        switch (direction)
        {
            case Direction.Up:    newRow--; break;
            case Direction.Down:  newRow++; break;
            case Direction.Left:  newCol--; break;
            case Direction.Right: newCol++; break;
        }

        if (newRow < 0 || newRow >= CurrentLevel.Height ||
            newCol < 0 || newCol >= CurrentLevel.Width)
            return;

        var target = CurrentLevel.Grid[newRow, newCol];

        if (target == Cell.Wall) return;

        if (target == Cell.Exit && !HasKey)
        {
            ExitBlockedFeedback = true;
            return;
        }

        Player.Y = newRow;
        Player.X = newCol;
        Player.Steps++;

        if (CurrentLevel.Grid[newRow, newCol] == Cell.Key)
        {
            HasKey = true;
            CurrentLevel.Grid[newRow, newCol] = Cell.Floor;
        }

        if (CurrentLevel.Grid[newRow, newCol] == Cell.Exit)
            IsCompleted = true;
    }

    private void PlaceKey(Cell[,] grid, int size)
    {
        int exitRow = size - 2;
        int exitCol = size - 2;

        // BFS from start — Exit counts as a wall (player can't pass without key)
        var distFromStart = BfsDistances(grid, 1, 1, size, treatExitAsWall: true);

        // BFS from exit outward — to measure how far cells are from the exit
        var distFromExit = BfsDistances(grid, exitRow, exitCol, size, treatExitAsWall: false);

        var candidates = new List<(int r, int c, int score)>();

        for (int r = 1; r < size - 1; r++)
        {
            for (int c = 1; c < size - 1; c++)
            {
                if (grid[r, c] != Cell.Floor) continue;
                if (distFromStart[r, c] < 0) continue;

                int score = distFromStart[r, c];
                if (distFromExit[r, c] >= 0) score += distFromExit[r, c];
                candidates.Add((r, c, score));
            }
        }

        if (candidates.Count == 0)
        {
            var fallback = BfsDistances(grid, 1, 1, size, treatExitAsWall: false);
            for (int r = 1; r < size - 1; r++)
                for (int c = 1; c < size - 1; c++)
                    if (grid[r, c] == Cell.Floor && fallback[r, c] >= 0)
                        candidates.Add((r, c, fallback[r, c]));
        }

        if (candidates.Count == 0) return;

        candidates.Sort((a, b) => b.score.CompareTo(a.score));

        int topCount = Math.Max(1, candidates.Count / 8);
        var (kr, kc, _) = candidates[_random.Next(topCount)];
        grid[kr, kc] = Cell.Key;
    }

    private static int[,] BfsDistances(Cell[,] grid, int startR, int startC, int size, bool treatExitAsWall)
    {
        var dist = new int[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                dist[r, c] = -1;

        var queue = new Queue<(int, int)>();
        queue.Enqueue((startR, startC));
        dist[startR, startC] = 0;

        int[] dRow = [-1, 1, 0, 0];
        int[] dCol = [0, 0, -1, 1];

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            for (int d = 0; d < 4; d++)
            {
                int nr = r + dRow[d];
                int nc = c + dCol[d];
                if (nr < 0 || nr >= size || nc < 0 || nc >= size) continue;
                if (dist[nr, nc] >= 0) continue;

                bool blocked = grid[nr, nc] == Cell.Wall
                               || (treatExitAsWall && grid[nr, nc] == Cell.Exit);

                if (!blocked)
                {
                    dist[nr, nc] = dist[r, c] + 1;
                    queue.Enqueue((nr, nc));
                }
            }
        }

        return dist;
    }
}
