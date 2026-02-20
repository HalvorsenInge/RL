using EldritchDungeon.Core;

namespace EldritchDungeon.World;

public class Tile
{
    public TileType Type { get; set; } = TileType.Wall;
    public bool IsExplored { get; set; }
    public bool IsInFov { get; set; }
    public TileEffect Effect { get; set; } = TileEffect.None;
    /// <summary>Turns remaining. -1 = permanent.</summary>
    public int EffectDuration { get; set; }
}
