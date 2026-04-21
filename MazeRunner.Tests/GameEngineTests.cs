using MazeRunner.Core.Interfaces;
using MazeRunner.Core.Models;
using MazeRunner.Core.Services;

namespace MazeRunner.Tests;

public class GameEngineTests
{
    private static GameEngine CreateEngine(int level = 1)
    {
        var engine = new GameEngine(new MazeGenerator(seed: 42));
        engine.LoadLevel(level);
        return engine;
    }

    [Fact]
    public void LoadLevel_PlayerStartsAtStartCell()
    {
        var engine = CreateEngine();

        Assert.Equal(1, engine.Player.X);
        Assert.Equal(1, engine.Player.Y);
    }

    [Fact]
    public void LoadLevel_PlayerStepCountIsZero()
    {
        var engine = CreateEngine();

        Assert.Equal(0, engine.Player.Steps);
    }

    [Fact]
    public void LoadLevel_IsCompletedIsFalse()
    {
        var engine = CreateEngine();

        Assert.False(engine.IsCompleted);
    }

    [Fact]
    public void LoadLevel_HasKeyIsFalseAtStart()
    {
        var engine = CreateEngine();

        Assert.False(engine.HasKey);
    }

    [Fact]
    public void LoadLevel_GridContainsExactlyOneKeyCell()
    {
        var engine = CreateEngine();
        var grid   = engine.CurrentLevel.Grid;
        int count  = 0;

        for (int r = 0; r < grid.GetLength(0); r++)
            for (int c = 0; c < grid.GetLength(1); c++)
                if (grid[r, c] == Cell.Key) count++;

        Assert.Equal(1, count);
    }

    [Fact]
    public void MovePlayer_IntoWall_PositionUnchanged()
    {
        var engine  = CreateEngine();
        int beforeX = engine.Player.X;
        int beforeY = engine.Player.Y;

        engine.MovePlayer(Direction.Up);

        Assert.Equal(beforeX, engine.Player.X);
        Assert.Equal(beforeY, engine.Player.Y);
    }

    [Fact]
    public void MovePlayer_IntoWall_StepsNotIncremented()
    {
        var engine = CreateEngine();

        engine.MovePlayer(Direction.Up);

        Assert.Equal(0, engine.Player.Steps);
    }

    [Fact]
    public void MovePlayer_IntoFloor_StepsIncremented()
    {
        var engine = CreateEngine();

        Direction? validDir = FindPassableDirection(engine);
        if (validDir is null) return;

        engine.MovePlayer(validDir.Value);
        Assert.Equal(1, engine.Player.Steps);
    }

    [Fact]
    public void MovePlayer_TowardExitWithoutKey_DoesNotComplete()
    {
        var engine = CreateEngine();

        Assert.False(engine.HasKey);

        var level   = engine.CurrentLevel;
        int exitRow = level.Height - 2;
        int exitCol = level.Width  - 2;

        engine.Player.X = exitCol;
        engine.Player.Y = exitRow - 1;

        engine.MovePlayer(Direction.Down);

        if (level.Grid[exitRow, exitCol] == Cell.Exit)
            Assert.False(engine.IsCompleted);
    }

    [Fact]
    public void MovePlayer_ExitBlocked_ExitBlockedFeedbackIsTrue()
    {
        var engine = CreateEngine();
        var level  = engine.CurrentLevel;
        int exitRow = level.Height - 2;
        int exitCol = level.Width  - 2;

        engine.Player.X = exitCol;
        engine.Player.Y = exitRow - 1;

        if (level.Grid[exitRow - 1, exitCol] != Cell.Wall)
        {
            engine.MovePlayer(Direction.Down);
            if (!engine.HasKey)
                Assert.True(engine.ExitBlockedFeedback);
        }
    }

    [Fact]
    public void LoadLevel_CurrentLevelHasCorrectNumber()
    {
        var engine = CreateEngine(level: 3);

        Assert.Equal(3, engine.CurrentLevel.LevelNumber);
    }

    [Fact]
    public void LoadLevel_GridSizeMatchesConfig()
    {
        for (int lvl = 1; lvl <= 6; lvl++)
        {
            var engine = new GameEngine(new MazeGenerator(seed: lvl));
            engine.LoadLevel(lvl);
            var expected = LevelConfig.GetLevel(lvl).GridSize;

            Assert.Equal(expected, engine.CurrentLevel.Width);
            Assert.Equal(expected, engine.CurrentLevel.Height);
        }
    }

    private static Direction? FindPassableDirection(GameEngine engine)
    {
        var level = engine.CurrentLevel;
        int px    = engine.Player.X;
        int py    = engine.Player.Y;

        var candidates = new (Direction dir, int row, int col)[]
        {
            (Direction.Up,    py - 1, px),
            (Direction.Down,  py + 1, px),
            (Direction.Left,  py,     px - 1),
            (Direction.Right, py,     px + 1)
        };

        foreach (var (dir, row, col) in candidates)
        {
            if (row >= 0 && row < level.Height && col >= 0 && col < level.Width &&
                level.Grid[row, col] != Cell.Wall && level.Grid[row, col] != Cell.Exit)
                return dir;
        }
        return null;
    }
}
