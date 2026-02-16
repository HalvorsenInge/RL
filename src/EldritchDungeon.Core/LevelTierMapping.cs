namespace EldritchDungeon.Core;

public static class LevelTierMapping
{
    public static (int MinTier, int MaxTier) GetTierRange(int dungeonLevel)
    {
        return dungeonLevel switch
        {
            >= 1 and <= 3 => (1, 2),
            >= 4 and <= 7 => (3, 4),
            >= 8 and <= 12 => (5, 6),
            >= 13 and <= 18 => (7, 8),
            >= 19 and <= 25 => (9, 10),
            _ => throw new ArgumentOutOfRangeException(nameof(dungeonLevel),
                $"Dungeon level must be between 1 and {GameConstants.MaxDungeonLevel}.")
        };
    }
}
