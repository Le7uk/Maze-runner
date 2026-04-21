namespace MazeRunner.Core.Session;

public static class SessionData
{
    private static readonly Dictionary<int, int> _bestStars = new()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 0 },
        { 4, 0 },
        { 5, 0 },
        { 6, 0 }
    };

    public static int GetBestStars(int levelNumber) =>
        _bestStars.TryGetValue(levelNumber, out var stars) ? stars : 0;

    public static void UpdateStars(int levelNumber, int stars)
    {
        if (_bestStars.ContainsKey(levelNumber) && stars > _bestStars[levelNumber])
            _bestStars[levelNumber] = stars;
    }

    public static void Reset()
    {
        foreach (var key in _bestStars.Keys.ToList())
            _bestStars[key] = 0;
    }
}
