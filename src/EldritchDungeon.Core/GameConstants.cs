namespace EldritchDungeon.Core;

public static class GameConstants
{
    // Screen dimensions
    public const int ScreenWidth = 80;
    public const int ScreenHeight = 25;

    // Stat rolling
    public const int StatDiceCount = 4;
    public const int StatDiceSides = 6;
    public const int StatDropCount = 1;
    public const int StatCount = 6;
    public const int MinStatTotal = 75;
    public const int MinSingleStat = 6;
    public const int MinStatValue = 3;
    public const int MaxStatValue = 18;

    // Sanity thresholds
    public const int MaxSanity = 100;
    public const int SanityStableMin = 51;
    public const int SanityFracturedMin = 26;
    public const int SanityUnravelingMin = 10;
    // Broken = 0..9

    // Addiction
    public const int MaxAddiction = 100;
    public const int AddictionThreshold = 50;

    // Religion
    public const int MaxFavor = 100;
    public const int MaxAnger = 100;
    public const int FavorTier1 = 25;
    public const int FavorTier2 = 50;
    public const int FavorTier3 = 75;
    public const int FavorTier4 = 100;

    // Dungeon
    public const int MaxDungeonLevel = 25;

    // Map generation
    public const int MapWidth = 80;
    public const int MapHeight = 21; // 4 rows reserved for HUD
    public const int MinRoomSize = 5;
    public const int MaxRoomSize = 13;
    public const int MaxRooms = 15;
    public const int DefaultFovRadius = 8;
    public const int MonstersPerRoomMin = 0;
    public const int MonstersPerRoomMax = 3;

    // Character glyphs
    public const char PlayerGlyph = '@';
    public const char WallGlyph = '#';
    public const char FloorGlyph = '.';
    public const char DoorGlyph = '+';
    public const char StairsDownGlyph = '>';
    public const char StairsUpGlyph = '<';
    public const char MonsterGlyphDefault = 'M';
}
