using EldritchDungeon.Core;

namespace EldritchDungeon.Utilities;

public static class Glyphs
{
    public static char ForTileType(TileType tileType)
    {
        return tileType switch
        {
            TileType.Wall => GameConstants.WallGlyph,
            TileType.Floor => GameConstants.FloorGlyph,
            TileType.Door => GameConstants.DoorGlyph,
            TileType.StairsDown => GameConstants.StairsDownGlyph,
            TileType.StairsUp => GameConstants.StairsUpGlyph,
            _ => '?'
        };
    }
}
