namespace EldritchDungeon.Utilities;

public static class ColorPalette
{
    public static readonly ConsoleColor Wall = ConsoleColor.DarkGray;
    public static readonly ConsoleColor Floor = ConsoleColor.Gray;
    public static readonly ConsoleColor Player = ConsoleColor.Yellow;
    public static readonly ConsoleColor Stairs = ConsoleColor.Cyan;
    public static readonly ConsoleColor Door = ConsoleColor.DarkYellow;
    public static readonly ConsoleColor ExploredWall = ConsoleColor.DarkBlue;
    public static readonly ConsoleColor ExploredFloor = ConsoleColor.DarkBlue;

    public static ConsoleColor MonsterByTier(int tier)
    {
        return tier switch
        {
            <= 2 => ConsoleColor.Green,
            <= 4 => ConsoleColor.DarkYellow,
            <= 6 => ConsoleColor.Red,
            <= 8 => ConsoleColor.Magenta,
            _ => ConsoleColor.DarkRed
        };
    }
}
