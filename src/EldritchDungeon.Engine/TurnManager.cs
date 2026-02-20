using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Engine;

public class TurnManager
{
    private readonly GameEngine _engine;

    public TurnManager(GameEngine engine)
    {
        _engine = engine;
    }

    public void ProcessTurn()
    {
        var player = _engine.Player;
        var map = _engine.Map;

        if (player == null || map == null)
            return;

        // 1. Status effects on player
        _engine.StatusEffectSystem.ProcessActor(player);

        // 2. Divine blessings (HP/mana regen, sanity resist) and wrath punishments
        _engine.ReligionSystem.ApplyBlessings(player);
        _engine.ReligionSystem.ApplyWrath(player);

        // 3. Check sanity from visible monsters
        _engine.SanitySystem.Update(map);

        // 4. AI moves/attacks monsters
        _engine.AISystem.Update(map);

        // 5. Addiction check
        _engine.AddictionSystem.Update(player);

        // 6. Tile effects — fire damage, expiry
        ProcessTileEffects(player, map);

        // 7. Update FOV
        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);

        // 8. Check player death
        if (player.Health.IsDead)
        {
            _engine.Log.Add("You have died! Game over.");
            _engine.OnPlayerDeath();
        }
    }

    private void ProcessTileEffects(Player player, DungeonMap map)
    {
        // Fire damage to the player if they're standing on a fire tile
        if (map.GetTileEffect(player.X, player.Y) == TileEffect.Fire)
        {
            player.Health.TakeDamage(5);
            _engine.Log.Add("You are on fire! (-5 HP)");
        }

        // Fire damage to monsters standing on fire tiles
        foreach (var monster in map.Monsters.ToList())
        {
            if (map.GetTileEffect(monster.X, monster.Y) == TileEffect.Fire)
            {
                monster.Health.TakeDamage(5);
                if (monster.Health.IsDead)
                {
                    _engine.Log.Add($"The {monster.Name} burns to death!");
                    map.Monsters.Remove(monster);
                    player.Stats.Experience += monster.XpValue;
                    _engine.LevelingSystem.CheckLevelUp(player);
                }
            }
        }

        // Tick down durations; expired effects are handled automatically in DungeonMap
        map.TickTileEffects();
    }
}
