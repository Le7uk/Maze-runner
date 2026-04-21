using MazeRunner.Core.Models;

namespace MazeRunner.Core.Interfaces;

public interface IGameEngine
{
    Level CurrentLevel { get; }
    Player Player { get; }
    bool IsCompleted { get; }
    bool HasKey { get; }
    bool ExitBlockedFeedback { get; }

    void MovePlayer(Direction direction);
    void LoadLevel(int levelNumber);
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
