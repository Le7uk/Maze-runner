using MazeRunner.Core.Services;

namespace MazeRunner.Tests;

public class LevelConfigTests
{
    [Fact]
    public void TotalLevels_IsExactlySix()
    {
        Assert.Equal(6, LevelConfig.TotalLevels);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void GetLevel_ValidNumber_ReturnsSettings(int levelNum)
    {
        var settings = LevelConfig.GetLevel(levelNum);

        Assert.NotNull(settings);
        Assert.Equal(levelNum, settings.LevelNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(-1)]
    public void GetLevel_OutOfRange_ThrowsArgumentOutOfRangeException(int levelNum)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LevelConfig.GetLevel(levelNum));
    }

    [Fact]
    public void GetAllLevels_CountIsSix()
    {
        Assert.Equal(6, LevelConfig.GetAllLevels().Count);
    }

    [Fact]
    public void LevelSettings_GridSizesAreOddAndIncreasing()
    {
        var all = LevelConfig.GetAllLevels();

        for (int i = 0; i < all.Count; i++)
        {
            Assert.Equal(1, all[i].GridSize % 2);

            if (i > 0)
                Assert.True(all[i].GridSize > all[i - 1].GridSize,
                    $"Level {i + 1} grid size must be larger than level {i}");
        }
    }

    [Fact]
    public void LevelSettings_ThreeStarThresholdIsLessThanTwoStar()
    {
        foreach (var s in LevelConfig.GetAllLevels())
        {
            Assert.True(s.ThreeStarSeconds < s.TwoStarSeconds,
                $"Level {s.LevelNumber}: 3-star threshold must be < 2-star threshold");
        }
    }

    [Fact]
    public void CalculateStars_ExactlyAtThreeStarThreshold_ReturnsThree()
    {
        var s = LevelConfig.GetLevel(1);

        Assert.Equal(3, LevelConfig.CalculateStars(1, s.ThreeStarSeconds));
    }

    [Fact]
    public void CalculateStars_BelowThreeStarThreshold_ReturnsThree()
    {
        var s = LevelConfig.GetLevel(1);

        Assert.Equal(3, LevelConfig.CalculateStars(1, s.ThreeStarSeconds - 1));
    }

    [Fact]
    public void CalculateStars_BetweenTwoAndThreeStarThresholds_ReturnsTwoStars()
    {
        var s = LevelConfig.GetLevel(1);

        Assert.Equal(2, LevelConfig.CalculateStars(1, s.ThreeStarSeconds + 1));
    }

    [Fact]
    public void CalculateStars_ExactlyAtTwoStarThreshold_ReturnsTwoStars()
    {
        var s = LevelConfig.GetLevel(1);

        Assert.Equal(2, LevelConfig.CalculateStars(1, s.TwoStarSeconds));
    }

    [Fact]
    public void CalculateStars_AboveTwoStarThreshold_ReturnsOneStar()
    {
        var s = LevelConfig.GetLevel(1);

        Assert.Equal(1, LevelConfig.CalculateStars(1, s.TwoStarSeconds + 1));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void CalculateStars_VeryLongTime_AlwaysReturnsAtLeastOneStar(int levelNum)
    {
        Assert.Equal(1, LevelConfig.CalculateStars(levelNum, 99999));
    }
}
