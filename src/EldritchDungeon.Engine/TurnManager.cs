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
        _engine.ReligionSystem.ApplyWrath(player, map);

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
        var rng = new Random();
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
                    AwardGoldFromFire(monster, player, rng);
                    _engine.LevelingSystem.CheckLevelUp(player);
                }
            }
        }

        // Tick down durations; expired effects are handled automatically in DungeonMap
        map.TickTileEffects();

        // Expire timed summons
        TickSummons(map);
    }

    private void TickSummons(DungeonMap map)
    {
        foreach (var monster in map.Monsters.ToList())
        {
            if (!monster.IsSummoned || monster.SummonDurationLeft < 0) continue;

            monster.SummonDurationLeft--;
            if (monster.SummonDurationLeft <= 0)
            {
                _engine.Log.Add($"The {monster.Name} fades back into the void.");
                map.Monsters.Remove(monster);
            }
        }
    }

    private void AwardGoldFromFire(Monster monster, Player player, Random rng)
    {
        if (monster.GoldDropChance <= 0) return;
        if (rng.NextDouble() > monster.GoldDropChance) return;

        int gold = monster.GoldMin >= monster.GoldMax
            ? monster.GoldMin
            : rng.Next(monster.GoldMin, monster.GoldMax + 1);
        if (gold <= 0) return;

        player.Gold += gold;

        if (monster.IsEldritchCoin)
        {
            player.Sanity.LoseSanity(1);
            _engine.Log.Add($"  You claim {gold} eldritch coins from the ashes. (-1 sanity)");
        }
        else
        {
            _engine.Log.Add($"  You find {gold} gold among the ashes.");
        }
    }
}
