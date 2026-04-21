using MazeRunner.Core.Models;
using MazeRunner.Core.Services;

namespace MazeRunner.Tests;

public class MazeGeneratorTests
{
    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(19)]
    [InlineData(23)]
    [InlineData(27)]
    [InlineData(31)]
    public void Generate_HasExactlyOneStartCell(int size)
    {
        var generator = new MazeGenerator(seed: 42);
        var grid = generator.Generate(size);

        int count = CountCells(grid, Cell.Start);
        Assert.Equal(1, count);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(19)]
    [InlineData(23)]
    [InlineData(27)]
    [InlineData(31)]
    public void Generate_HasExactlyOneExitCell(int size)
    {
        var generator = new MazeGenerator(seed: 42);
        var grid = generator.Generate(size);

        int count = CountCells(grid, Cell.Exit);
        Assert.Equal(1, count);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(19)]
    public void Generate_StartIsAtTopLeft(int size)
    {
        var generator = new MazeGenerator(seed: 42);
        var grid = generator.Generate(size);

        Assert.Equal(Cell.Start, grid[1, 1]);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(19)]
    public void Generate_ExitIsAtBottomRight(int size)
    {
        var generator = new MazeGenerator(seed: 42);
        var grid = generator.Generate(size);

        Assert.Equal(Cell.Exit, grid[size - 2, size - 2]);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(21)]
    public void Generate_AllFloorsReachableFromStart(int size)
    {
        var generator = new MazeGenerator(seed: 99);
        var grid = generator.Generate(size);

        bool allReachable = MazeGenerator.AreAllFloorsReachable(grid);
        Assert.True(allReachable, $"Not all floor cells are reachable in a {size}x{size} maze.");
    }

    [Fact]
    public void Generate_EvenSizeIsNormalisedToOdd()
    {
        var generator = new MazeGenerator(seed: 1);
        var grid = generator.Generate(10);   // 10 → should become 11

        // 10 is even, algorithm bumps it to 11
        Assert.Equal(11, grid.GetLength(0));
        Assert.Equal(11, grid.GetLength(1));
    }

    [Fact]
    public void Generate_BorderCellsAreAlwaysWalls()
    {
        var generator = new MazeGenerator(seed: 7);
        var grid = generator.Generate(11);
        int size = grid.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            Assert.Equal(Cell.Wall, grid[0, i]);       // top row
            Assert.Equal(Cell.Wall, grid[size - 1, i]); // bottom row
            Assert.Equal(Cell.Wall, grid[i, 0]);       // left column
            Assert.Equal(Cell.Wall, grid[i, size - 1]); // right column
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static int CountCells(Cell[,] grid, Cell target)
    {
        int count = 0;
        for (int r = 0; r < grid.GetLength(0); r++)
            for (int c = 0; c < grid.GetLength(1); c++)
                if (grid[r, c] == target) count++;
        return count;
    }
}
