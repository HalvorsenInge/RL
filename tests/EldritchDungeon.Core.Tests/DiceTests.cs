using EldritchDungeon.Core;

namespace EldritchDungeon.Core.Tests;

public class DiceTests
{
    [Fact]
    public void Roll_SingleD6_ReturnsBetween1And6()
    {
        for (int i = 0; i < 100; i++)
        {
            int result = Dice.Roll(1, 6);
            Assert.InRange(result, 1, 6);
        }
    }

    [Fact]
    public void Roll_2d6_ReturnsBetween2And12()
    {
        for (int i = 0; i < 100; i++)
        {
            int result = Dice.Roll(2, 6);
            Assert.InRange(result, 2, 12);
        }
    }

    [Fact]
    public void RollDropLowest_4d6Drop1_ReturnsBetween3And18()
    {
        for (int i = 0; i < 100; i++)
        {
            int result = Dice.RollDropLowest(4, 6, 1);
            Assert.InRange(result, 3, 18);
        }
    }

    [Fact]
    public void RollAbilityScores_ReturnsValidArray()
    {
        var scores = Dice.RollAbilityScores();

        Assert.Equal(6, scores.Length);
        Assert.True(scores.Sum() >= GameConstants.MinStatTotal,
            $"Total {scores.Sum()} should be >= {GameConstants.MinStatTotal}");
        Assert.All(scores, s => Assert.True(s >= GameConstants.MinSingleStat,
            $"Stat {s} should be >= {GameConstants.MinSingleStat}"));
    }

    [Fact]
    public void RollAbilityScores_AllScoresInValidRange()
    {
        for (int i = 0; i < 20; i++)
        {
            var scores = Dice.RollAbilityScores();
            Assert.All(scores, s => Assert.InRange(s, 3, 18));
        }
    }

    [Theory]
    [InlineData(10, 0)]  // 10 -> +0
    [InlineData(12, 1)]  // 12 -> +1
    [InlineData(8, -1)]  // 8  -> -1
    [InlineData(18, 4)]  // 18 -> +4
    [InlineData(3, -4)]  // 3  -> -4
    [InlineData(1, -5)]  // 1  -> -5
    [InlineData(11, 0)]  // 11 -> +0
    [InlineData(9, -1)]  // 9  -> -1
    public void GetModifier_ReturnsCorrectValue(int score, int expectedModifier)
    {
        Assert.Equal(expectedModifier, Dice.GetModifier(score));
    }

    [Fact]
    public void SetSeed_ProducesDeterministicResults()
    {
        Dice.SetSeed(42);
        int first = Dice.Roll(1, 6);

        Dice.SetSeed(42);
        int second = Dice.Roll(1, 6);

        Assert.Equal(first, second);
    }

    [Fact]
    public void RollAbilityScores_NeverBelowMinimums()
    {
        for (int i = 0; i < 50; i++)
        {
            var scores = Dice.RollAbilityScores();
            Assert.True(scores.Sum() >= GameConstants.MinStatTotal);
            Assert.All(scores, s => Assert.True(s >= GameConstants.MinSingleStat));
        }
    }
}
