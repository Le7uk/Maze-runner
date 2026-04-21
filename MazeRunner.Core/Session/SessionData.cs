namespace MazeRunner.Core.Session;

/// <summary>
/// Stores session-only star ratings. Data is lost when the application closes.
/// </summary>
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

    /// <summary>
    /// Returns the best star count (0–3) achieved for the given level this session.
    /// </summary>
    public static int GetBestStars(int levelNumber) =>
        _bestStars.TryGetValue(levelNumber, out var stars) ? stars : 0;

    /// <summary>
    /// Updates the best star count for a level if the new result is better.
    /// </summary>
    public static void UpdateStars(int levelNumber, int stars)
    {
        if (_bestStars.ContainsKey(levelNumber) && stars > _bestStars[levelNumber])
            _bestStars[levelNumber] = stars;
    }

    /// <summary>
    /// Resets all star data (e.g. when starting a new session from the menu).
    /// </summary>
    public static void Reset()
    {
        foreach (var key in _bestStars.Keys.ToList())
            _bestStars[key] = 0;
    }
}
