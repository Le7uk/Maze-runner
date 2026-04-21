using MazeRunner.Core.Interfaces;
using MazeRunner.Core.Models;

namespace MazeRunner.Core.Services;

/// <summary>
/// Core game logic: loads levels, moves the player, and detects level completion.
/// </summary>
public class GameEngine : IGameEngine
{
    private readonly MazeGenerator _generator;

    public Level CurrentLevel { get; private set; } = null!;
    public Player Player { get; private set; } = null!;
    public bool IsCompleted { get; private set; }

    public GameEngine(MazeGenerator? generator = null)
    {
        _generator = generator ?? new MazeGenerator();
    }

    /// <summary>
    /// Loads (generates) a level by number and places the player at the Start cell.
    /// </summary>
    public void LoadLevel(int levelNumber)
    {
        var settings = LevelConfig.GetLevel(levelNumber);
        var grid = _generator.Generate(settings.GridSize);
        CurrentLevel = new Level(levelNumber, grid);

        // Player starts at (row=1, col=1) — the Start cell
        Player = new Player(1, 1);
        IsCompleted = false;
    }

    /// <summary>
    /// Attempts to move the player in the given direction.
    /// Movement is blocked by Wall cells and ignored if the level is already completed.
    /// </summary>
    public void MovePlayer(Direction direction)
    {
        if (IsCompleted) return;

        int newRow = Player.Y;
        int newCol = Player.X;

        switch (direction)
        {
            case Direction.Up:    newRow--; break;
            case Direction.Down:  newRow++; break;
            case Direction.Left:  newCol--; break;
            case Direction.Right: newCol++; break;
        }

        // Bounds check
        if (newRow < 0 || newRow >= CurrentLevel.Height ||
            newCol < 0 || newCol >= CurrentLevel.Width)
            return;

        // Wall check
        if (CurrentLevel.Grid[newRow, newCol] == Cell.Wall)
            return;

        // Move the player
        Player.Y = newRow;
        Player.X = newCol;
        Player.Steps++;

        // Check if the player reached the Exit
        if (CurrentLevel.Grid[newRow, newCol] == Cell.Exit)
            IsCompleted = true;
    }
}
