using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public abstract class GameSystem
{
    public abstract void Update(DungeonMap map);
}
