namespace MazeRunner.Core.Models;

public class Level
{
    public int LevelNumber { get; init; }
    public Cell[,] Grid { get; init; }
    public int Width => Grid.GetLength(1);
    public int Height => Grid.GetLength(0);

    public Level(int levelNumber, Cell[,] grid)
    {
        LevelNumber = levelNumber;
        Grid = grid;
    }
}
