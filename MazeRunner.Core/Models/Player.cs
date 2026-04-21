namespace MazeRunner.Core.Models;

public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Steps { get; set; }

    public Player(int x, int y)
    {
        X = x;
        Y = y;
        Steps = 0;
    }
}
