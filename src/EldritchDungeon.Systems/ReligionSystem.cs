using EldritchDungeon.Core;
using EldritchDungeon.Data.Gods;
using EldritchDungeon.Data.Monsters;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Components;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class ReligionSystem
{
    private readonly Action<string> _log;
    private readonly Random _random = new();

    // Cooldown in turns between god summon waves
    private const int SummonCooldownTurns = 30;

    public ReligionSystem(Action<string> log)
    {
        _log = log;
    }

    // ── Kill reactions ────────────────────────────────────────────────────────

    /// <summary>
    /// Called every time the player slays a monster.
    /// Gods react regardless of whether the player follows them.
    /// If the player has no CurrentGod, the most interested god may claim them.
    /// </summary>
    public void OnKill(Player player, Monster monster)
    {
        // Check every god's reaction (gods watch regardless of worship)
        foreach (var (godType, god) in GodDatabase.GetAll())
        {
            bool isLovedKill  = god.LovedMonsters.Contains(monster.Name);
            bool isHatedKill  = god.HatedMonsters.Contains(monster.Name);

            if (!isLovedKill && !isHatedKill) continue;

            if (player.Religion.CurrentGod == null)
            {
                // A god who cares about this kill takes notice and claims the player
                player.Religion.SetGod(godType);
                if (isLovedKill)
                {
                    player.Religion.AddAnger(god.AngerOnLovedKill);
                    _log($"[{god.Name}] notices you slaying his {monster.Name}. ANGER rises! ({god.Name} has claimed you)");
                }
                else
                {
                    player.Religion.AddFavor(god.FavorOnHatedKill);
                    _log($"[{god.Name}] is pleased by the death of the {monster.Name}. FAVOR rises! ({god.Name} has claimed you)");
                }
                return; // Only one god claims per kill
            }

            if (player.Religion.CurrentGod == godType)
            {
                if (isLovedKill)
                {
                    player.Religion.AddAnger(god.AngerOnLovedKill);
                    _log($"[{god.Name}] You have slain {god.Name}'s beloved {monster.Name}! ANGER +{god.AngerOnLovedKill}");
                }
                else
                {
                    player.Religion.AddFavor(god.FavorOnHatedKill);
                    _log($"[{god.Name}] {god.Name} is pleased by the death of the {monster.Name}. FAVOR +{god.FavorOnHatedKill}");
                }
            }
        }

        // Minor passive favor from current god for any kill
        if (player.Religion.CurrentGod.HasValue)
        {
            var currentGod = GodDatabase.Get(player.Religion.CurrentGod.Value);
            bool alreadyReacted = currentGod.LovedMonsters.Contains(monster.Name)
                                  || currentGod.HatedMonsters.Contains(monster.Name);
            if (!alreadyReacted)
                player.Religion.AddFavor(1);
        }

        // Anger decays 1 point every 5 kills (gods forget slowly)
        if (_random.Next(5) == 0 && player.Religion.Anger > 0)
            player.Religion.DecreaseAnger(1);
    }

    // ── Per-turn effects ──────────────────────────────────────────────────────

    /// <summary>
    /// Called each turn. Applies per-turn blessings (HP/mana regen, sanity resist).
    /// </summary>
    public void ApplyBlessings(Player player)
    {
        if (player.Religion.CurrentGod == null || player.Religion.PowerTier == 0)
        {
            player.Sanity.InsanityResist = 0.0;
            return;
        }

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        var blessing = god.Blessing;
        if (blessing == null) return;

        double value = blessing.BaseValuePerTier * player.Religion.PowerTier;

        switch (blessing.Type)
        {
            case BlessingType.HpRegenPerTurn:
                if (!player.Health.IsDead)
                    player.Health.Heal((int)value);
                break;
            case BlessingType.ManaRegenPerTurn:
                player.Mana.Restore((int)value);
                break;
            case BlessingType.SanityResistBonus:
                player.Sanity.InsanityResist = value;
                break;
        }

        if (blessing.Type != BlessingType.SanityResistBonus)
            player.Sanity.InsanityResist = 0.0;
    }

    /// <summary>
    /// Called each turn. Fires anger-based wrath effects and, at high anger, god summon waves.
    /// </summary>
    public void ApplyWrath(Player player, DungeonMap? map = null)
    {
        if (player.Religion.CurrentGod == null || player.Religion.Anger == 0)
        {
            if (player.Religion.SummonCooldown > 0)
                player.Religion.SummonCooldown--;
            return;
        }

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        int anger = player.Religion.Anger;

        // Existing per-turn wrath effect (HP/sanity/status damage)
        var wrathEffect = god.WrathEffects
            .Where(e => anger >= e.AngerThreshold)
            .MaxBy(e => e.AngerThreshold);

        if (wrathEffect != null && _random.NextDouble() < wrathEffect.TriggerChance)
        {
            if (wrathEffect.HpDamage > 0)     player.Health.TakeDamage(wrathEffect.HpDamage);
            if (wrathEffect.SanityDamage > 0)  player.Sanity.LoseSanity(wrathEffect.SanityDamage);
            if (wrathEffect.AppliedStatus.HasValue)
                player.StatusEffects.AddEffect(wrathEffect.AppliedStatus.Value, wrathEffect.StatusDuration);
            _log(wrathEffect.Message);
        }

        // Monster summon waves
        if (map != null && player.Religion.SummonCooldown <= 0 && god.SummonWaves.Count > 0)
        {
            var summonWave = god.SummonWaves
                .Where(w => anger >= w.AngerThreshold)
                .MaxBy(w => w.AngerThreshold);

            if (summonWave != null && _random.NextDouble() < summonWave.TriggerChance)
            {
                SpawnWave(summonWave, player, map);
                player.Religion.SummonCooldown = SummonCooldownTurns;
            }
        }

        if (player.Religion.SummonCooldown > 0)
            player.Religion.SummonCooldown--;

        // Anger very slowly decays (1 per ~100 turns)
        if (_random.Next(100) == 0 && player.Religion.Anger > 0)
            player.Religion.DecreaseAnger(1);
    }

    // ── Monster summoning ─────────────────────────────────────────────────────

    private void SpawnWave(Data.Gods.GodSummonWave wave, Player player, DungeonMap map)
    {
        _log(wave.Message);
        int spawned = 0;

        foreach (var (name, count) in wave.Monsters)
        {
            if (!MonsterDatabase.GetAll().TryGetValue(name, out var def) || def == null) continue;

            for (int i = 0; i < count; i++)
            {
                var (sx, sy) = FindSpawnTile(player, map);
                if (sx < 0) break;

                var monster = new Monster
                {
                    Name         = def.Name,
                    Glyph        = def.Glyph,
                    Tier         = def.Tier,
                    Damage       = def.Damage,
                    XpValue      = def.XpValue,
                    SanityDamage = def.SanityDamage,
                    GoldMin      = def.GoldMin,
                    GoldMax      = def.GoldMax,
                    GoldDropChance = def.GoldDropChance,
                    IsEldritchCoin = def.IsEldritchCoin,
                };
                monster.Health.MaxHp     = def.HP;
                monster.Health.CurrentHp = def.HP;
                map.PlaceActor(monster, sx, sy);
                spawned++;
            }
        }

        if (spawned > 0)
            _log($"  {spawned} creature(s) materialise from the darkness!");
    }

    private (int x, int y) FindSpawnTile(Player player, DungeonMap map)
    {
        // Try to find a floor tile within 6–12 tiles of the player that is walkable
        for (int attempt = 0; attempt < 30; attempt++)
        {
            int dist = _random.Next(4, 12);
            int angle = _random.Next(360);
            int dx = (int)(Math.Cos(angle * Math.PI / 180) * dist);
            int dy = (int)(Math.Sin(angle * Math.PI / 180) * dist);
            int x = player.X + dx;
            int y = player.Y + dy;

            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height) continue;
            if (map.IsWalkable(x, y)) return (x, y);
        }
        return (-1, -1);
    }

    // ── Static helpers ────────────────────────────────────────────────────────

    public static double GetBlessingValue(Player player, BlessingType type)
    {
        if (player.Religion.CurrentGod == null || player.Religion.PowerTier == 0)
            return 0.0;

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        if (god.Blessing?.Type != type) return 0.0;

        return god.Blessing.BaseValuePerTier * player.Religion.PowerTier;
    }
}
