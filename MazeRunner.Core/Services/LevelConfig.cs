namespace MazeRunner.Core.Services;

public record LevelSettings(int LevelNumber, int GridSize, int ThreeStarSeconds, int TwoStarSeconds);

public static class LevelConfig
{
    private static readonly LevelSettings[] _levels =
    [
        new(1, 11, 30,  60),
        new(2, 15, 45,  90),
        new(3, 19, 60,  120),
        new(4, 23, 90,  180),
        new(5, 27, 120, 240),
        new(6, 31, 150, 300),
    ];

    public static int TotalLevels => _levels.Length;

    public static LevelSettings GetLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > _levels.Length)
            throw new ArgumentOutOfRangeException(nameof(levelNumber), "Level number must be between 1 and 6.");

        return _levels[levelNumber - 1];
    }

    public static IReadOnlyList<LevelSettings> GetAllLevels() => _levels;

    public static int CalculateStars(int levelNumber, int elapsedSeconds)
    {
        var settings = GetLevel(levelNumber);

        if (elapsedSeconds <= settings.ThreeStarSeconds) return 3;
        if (elapsedSeconds <= settings.TwoStarSeconds)   return 2;
        return 1;
    }
}
