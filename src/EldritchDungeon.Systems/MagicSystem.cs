using EldritchDungeon.Core;
using EldritchDungeon.Data.Spells;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class MagicSystem
{
    private readonly Action<string> _log;

    public MagicSystem(Action<string> log)
    {
        _log = log;
    }

    // ── Entry point ─────────────────────────────────────────────────────────

    /// <summary>
    /// Attempts to cast <paramref name="spellId"/> at (targetX, targetY).
    /// Returns true if a turn was consumed (successful cast or resisted attempt).
    /// </summary>
    public bool CastSpell(Player player, SpellId spellId, int targetX, int targetY, DungeonMap map)
    {
        var spell = SpellDatabase.Get(spellId);

        if (player.Sanity.State == SanityState.Broken)
        {
            _log("Your shattered mind cannot hold a spell!");
            return false;
        }

        if (!player.Mana.Spend(spell.ManaCost))
        {
            _log($"Not enough mana to cast {spell.Name}! (Need {spell.ManaCost} MP)");
            return false;
        }

        if (spell.IsSuperSpell)
            _log($"[SUPER SPELL] You invoke {spell.Name.ToUpper()}!");
        else
            _log($"You cast {spell.Name}!");

        switch (spell.Id)
        {
            case SpellId.MageArmor:
                CastMageArmor(player, spell);
                break;

            case SpellId.Teleport:
                CastTeleport(player, targetX, targetY, map);
                break;

            case SpellId.DrainLife:
                CastDrainLife(player, targetX, targetY, map, spell);
                break;

            case SpellId.ChainLightning:
                CastChainLightning(player, targetX, targetY, map, spell);
                break;

            case SpellId.CreateWater:
                CastCreateWater(targetX, targetY, map, spell);
                break;

            case SpellId.EyeInTheSky:
                CastEyeInTheSky(map);
                break;

            default:
                CastGenericSpell(spell, player, targetX, targetY, map);
                break;
        }

        return true;
    }

    // ── Spell implementations ────────────────────────────────────────────────

    private void CastMageArmor(Player player, SpellDefinition spell)
    {
        player.StatusEffects.AddEffect(StatusEffectType.Blessed, spell.StatusDuration, 0);
        _log("Arcane force wraps around you. (+AC for 20 turns)");
    }

    private void CastTeleport(Player player, int targetX, int targetY, DungeonMap map)
    {
        var tile = map.GetTile(targetX, targetY);
        if (tile.Type == TileType.Wall || !map.GetCell(targetX, targetY).IsWalkable
            || map.GetMonsterAt(targetX, targetY) != null)
        {
            _log("You cannot teleport there!");
            // Mana was already spent — turn still taken.
            return;
        }
        map.TryMoveActor(player, targetX, targetY);
        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);
        _log("Reality folds — you blink across the dungeon.");
    }

    private void CastDrainLife(Player player, int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        var monster = map.GetMonsterAt(targetX, targetY);
        if (monster == null) { _log("No target there."); return; }

        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int dmg = Math.Max(1, spell.BaseDamage + intMod);
        monster.Health.TakeDamage(dmg);

        int healed = dmg / 2;
        player.Health.Heal(healed);
        _log($"You drain {dmg} life from the {monster.Name}, recovering {healed} HP.");

        CheckMonsterDeath(monster, player, map);
    }

    private void CastChainLightning(Player player, int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int baseDmg = Math.Max(1, spell.BaseDamage + intMod);

        var hitTargets = new HashSet<Monster>();

        // Primary target
        var primary = map.GetMonsterAt(targetX, targetY);
        if (primary == null) { _log("No target there."); return; }

        ApplyDamageToMonster(primary, baseDmg, player, map);
        hitTargets.Add(primary);

        // Arc to up to 2 nearby monsters within 3 tiles
        int arcs = 2;
        var lastTarget = primary;
        for (int a = 0; a < arcs; a++)
        {
            var next = map.Monsters
                .Where(m => !hitTargets.Contains(m) && !m.Health.IsDead)
                .Where(m => Math.Max(Math.Abs(m.X - lastTarget.X), Math.Abs(m.Y - lastTarget.Y)) <= 3)
                .OrderBy(m => Math.Max(Math.Abs(m.X - lastTarget.X), Math.Abs(m.Y - lastTarget.Y)))
                .FirstOrDefault();

            if (next == null) break;

            int arcDmg = Math.Max(1, (int)(baseDmg * Math.Pow(0.6, a + 1)));
            _log($"Lightning arcs to the {next.Name} for {arcDmg} damage!");
            ApplyDamageToMonster(next, arcDmg, player, map);
            hitTargets.Add(next);
            lastTarget = next;
        }

        // Terrain: lightning on water at primary tile
        HandleLightningOnTerrain(targetX, targetY, player, map);
    }

    private void CastCreateWater(int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        int created = 0;
        for (int dx = -spell.Radius; dx <= spell.Radius; dx++)
        {
            for (int dy = -spell.Radius; dy <= spell.Radius; dy++)
            {
                int tx = targetX + dx;
                int ty = targetY + dy;
                if (tx < 0 || tx >= map.Width || ty < 0 || ty >= map.Height) continue;
                var tile = map.GetTile(tx, ty);
                if (tile.Type == TileType.Wall) continue;

                var existing = map.GetTileEffect(tx, ty);
                if (existing == TileEffect.Fire)
                {
                    map.SetTileEffect(tx, ty, TileEffect.None, 0);
                    _log("Water extinguishes the fire!");
                }
                else if (existing == TileEffect.None)
                {
                    map.SetTileEffect(tx, ty, TileEffect.Water, -1); // permanent
                    created++;
                }
            }
        }
        if (created > 0)
            _log($"Water floods {created} tiles. It flows through the dungeon...");
    }

    private void CastEyeInTheSky(DungeonMap map)
    {
        _log("=== EYE IN THE SKY REPORT ===");

        // Monsters
        if (map.Monsters.Count == 0)
        {
            _log("[Monsters] None remaining on this level.");
        }
        else
        {
            _log($"[Monsters] {map.Monsters.Count} remaining:");
            // Group by name for compact display
            var groups = map.Monsters
                .GroupBy(m => m.Name)
                .OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                if (group.Count() == 1)
                {
                    var m = group.First();
                    _log($"  - {m.Name} ({m.Health.CurrentHp}/{m.Health.MaxHp} HP) at ({m.X},{m.Y})");
                }
                else
                {
                    _log($"  - {group.Key} x{group.Count()}:");
                    foreach (var m in group)
                        _log($"      HP:{m.Health.CurrentHp}/{m.Health.MaxHp} at ({m.X},{m.Y})");
                }
            }
        }

        // Items
        if (map.Items.Count == 0)
        {
            _log("[Items] None remaining on this level.");
        }
        else
        {
            _log($"[Items] {map.Items.Count} on this level:");
            foreach (var (item, ix, iy) in map.Items)
                _log($"  - {item.Name} at ({ix},{iy})");
        }

        _log("=== END OF REPORT === (Press M to review)");
    }

    // ── Generic spell dispatch ───────────────────────────────────────────────

    private void CastGenericSpell(SpellDefinition spell, Player player, int targetX, int targetY, DungeonMap map)
    {
        int intMod = player.Stats.GetModifier(StatType.Intelligence);

        if (spell.Target == SpellTarget.SingleTarget)
        {
            var monster = map.GetMonsterAt(targetX, targetY);
            if (monster == null) { _log("No target there."); return; }

            int dmg = Math.Max(1, spell.BaseDamage + intMod);
            ApplyDamageToMonster(monster, dmg, player, map);
            ApplyStatusToMonster(monster, spell);
            HandleSpellTerrainInteraction(spell, targetX, targetY, player, map);
        }
        else if (spell.Target == SpellTarget.Area)
        {
            bool hitAny = false;
            for (int dx = -spell.Radius; dx <= spell.Radius; dx++)
            {
                for (int dy = -spell.Radius; dy <= spell.Radius; dy++)
                {
                    int tx = targetX + dx;
                    int ty = targetY + dy;
                    var monster = map.GetMonsterAt(tx, ty);
                    if (monster != null)
                    {
                        int dmg = Math.Max(1, spell.BaseDamage + intMod);
                        ApplyDamageToMonster(monster, dmg, player, map);
                        ApplyStatusToMonster(monster, spell);
                        hitAny = true;
                    }
                    HandleSpellTerrainInteraction(spell, tx, ty, player, map);
                }
            }
            if (!hitAny && spell.BaseDamage > 0)
                _log("The spell hits no enemies.");

            // Meteor leaves fire at impact site
            if (spell.Id == SpellId.Meteor)
                map.SetTileEffect(targetX, targetY, TileEffect.Fire, 10);
        }
    }

    // ── Terrain interactions ─────────────────────────────────────────────────

    private void HandleSpellTerrainInteraction(SpellDefinition spell, int x, int y, Player player, DungeonMap map)
    {
        if (spell.DamageType == DamageType.Fire)
            HandleFireOnTerrain(x, y, player, map);
        else if (spell.DamageType == DamageType.Lightning)
            HandleLightningOnTerrain(x, y, player, map);
        else if (spell.DamageType == DamageType.Cold)
            HandleColdOnTerrain(x, y, map);
    }

    private void HandleFireOnTerrain(int x, int y, Player player, DungeonMap map)
    {
        var effect = map.GetTileEffect(x, y);
        switch (effect)
        {
            case TileEffect.Water:
                // Steam cloud — radius 1, 5 turns, blocks LOS
                _log("Fire meets water — steam erupts!");
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    map.SetTileEffect(x + dx, y + dy, TileEffect.Steam, 5);
                break;

            case TileEffect.Oil:
                // Oil explosion — extra damage radius 2
                _log("The oil ignites — EXPLOSION!");
                for (int dx = -2; dx <= 2; dx++)
                for (int dy = -2; dy <= 2; dy++)
                {
                    var m = map.GetMonsterAt(x + dx, y + dy);
                    if (m != null)
                    {
                        m.Health.TakeDamage(35);
                        _log($"The explosion engulfs the {m.Name} for 35 fire damage!");
                        CheckMonsterDeath(m, player, map);
                    }
                }
                map.SetTileEffect(x, y, TileEffect.Fire, 10);
                break;

            case TileEffect.None:
                // Leave a fire tile behind
                map.SetTileEffect(x, y, TileEffect.Fire, 8);
                break;
        }
    }

    private void HandleLightningOnTerrain(int x, int y, Player player, DungeonMap map)
    {
        var effect = map.GetTileEffect(x, y);
        switch (effect)
        {
            case TileEffect.Water:
                _log("Lightning conducts through the water — electric shock!");
                var waterTiles = map.GetConnectedWaterTiles(x, y);
                foreach (var (wx, wy) in waterTiles)
                {
                    var m = map.GetMonsterAt(wx, wy);
                    if (m != null)
                    {
                        int shockDmg = Dice.Roll(1, 8) + 5;
                        m.Health.TakeDamage(shockDmg);
                        _log($"The {m.Name} is electrocuted for {shockDmg} damage!");
                        CheckMonsterDeath(m, player, map);
                    }
                }
                break;

            case TileEffect.Oil:
                _log("Lightning ignites the oil!");
                map.SetTileEffect(x, y, TileEffect.Fire, 10);
                break;
        }
    }

    private void HandleColdOnTerrain(int x, int y, DungeonMap map)
    {
        if (map.GetTileEffect(x, y) == TileEffect.Fire)
        {
            map.SetTileEffect(x, y, TileEffect.None, 0);
            _log("The ice extinguishes the flames.");
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void ApplyDamageToMonster(Monster monster, int damage, Player player, DungeonMap map)
    {
        monster.Health.TakeDamage(damage);
        _log($"The {monster.Name} takes {damage} damage.");
        CheckMonsterDeath(monster, player, map);
    }

    private void ApplyStatusToMonster(Monster monster, SpellDefinition spell)
    {
        if (spell.AppliedStatus.HasValue && !monster.Health.IsDead)
        {
            monster.StatusEffects.AddEffect(spell.AppliedStatus.Value, spell.StatusDuration, spell.BaseDamage / 4);
            _log($"The {monster.Name} is {spell.AppliedStatus.Value.ToString().ToLower()}!");
        }
    }

    private void CheckMonsterDeath(Monster monster, Player player, DungeonMap map)
    {
        if (monster.Health.IsDead && map.Monsters.Contains(monster))
        {
            _log($"The {monster.Name} is slain! (+{monster.XpValue} XP)");
            map.Monsters.Remove(monster);
            player.Stats.Experience += monster.XpValue;
        }
    }
}
