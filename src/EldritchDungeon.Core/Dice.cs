namespace EldritchDungeon.Core;

public static class Dice
{
    private static Random _random = new();

    /// <summary>
    /// Seeds the random number generator for deterministic testing.
    /// </summary>
    public static void SetSeed(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>
    /// Roll n dice with the given number of sides, return total.
    /// </summary>
    public static int Roll(int count, int sides)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
            total += _random.Next(1, sides + 1);
        return total;
    }

    /// <summary>
    /// Roll count dice, drop the lowest dropCount, return total of remaining.
    /// </summary>
    public static int RollDropLowest(int count, int sides, int dropCount)
    {
        var rolls = new List<int>(count);
        for (int i = 0; i < count; i++)
            rolls.Add(_random.Next(1, sides + 1));

        rolls.Sort();
        int total = 0;
        for (int i = dropCount; i < rolls.Count; i++)
            total += rolls[i];
        return total;
    }

    /// <summary>
    /// Roll a single ability score: 4d6 drop lowest 1.
    /// </summary>
    public static int RollAbilityScore()
    {
        return RollDropLowest(GameConstants.StatDiceCount, GameConstants.StatDiceSides, GameConstants.StatDropCount);
    }

    /// <summary>
    /// Roll a full set of 6 ability scores. Rerolls if total &lt; 75 or any stat &lt; 6.
    /// </summary>
    public static int[] RollAbilityScores()
    {
        while (true)
        {
            var scores = new int[GameConstants.StatCount];
            int total = 0;
            bool valid = true;

            for (int i = 0; i < GameConstants.StatCount; i++)
            {
                scores[i] = RollAbilityScore();
                total += scores[i];
                if (scores[i] < GameConstants.MinSingleStat)
                    valid = false;
            }

            if (valid && total >= GameConstants.MinStatTotal)
                return scores;
        }
    }

    /// <summary>
    /// Calculate ability modifier: (score - 10) / 2, rounded down.
    /// </summary>
    public static int GetModifier(int abilityScore)
    {
        return (int)Math.Floor((abilityScore - 10) / 2.0);
    }
}
