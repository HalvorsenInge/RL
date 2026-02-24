using EldritchDungeon.Core;
using EldritchDungeon.Data.Monsters;
using EldritchDungeon.Data.Spells;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class MagicSystem
{
    private readonly Action<string> _log;
    private readonly Random _random = new();

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

            case SpellId.Blink:
                CastBlink(player, map);
                break;

            case SpellId.ShadowStep:
                CastShadowStep(player, targetX, targetY, map);
                break;

            case SpellId.DeathWord:
                CastDeathWord(player, targetX, targetY, map);
                break;

            case SpellId.EldritchDrain:
                CastEldritchDrain(player, targetX, targetY, map, spell);
                break;

            case SpellId.RaiseDead:
                CastRaiseDead(player, targetX, targetY, map);
                break;

            case SpellId.BoneSpear:
                CastBoneSpear(player, targetX, targetY, map, spell);
                break;

            case SpellId.ArmageddonRain:
                CastArmageddonRain(player, map, spell);
                break;

            case SpellId.MassPetrification:
                CastMassPetrification(map, spell);
                break;

            case SpellId.TheDreamingWord:
                CastTheDreamingWord(player, map);
                break;

            case SpellId.SummonHorde:
                CastSummonHorde(player, map);
                break;

            case SpellId.RealityFracture:
                CastRealityFracture(player, targetX, targetY, map);
                break;

            case SpellId.VoidCollapse:
                CastVoidCollapse(player, targetX, targetY, map, spell);
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
            AwardGold(monster, player);
        }
    }

    // ── New spell implementations ────────────────────────────────────────────

    private void CastBlink(Player player, DungeonMap map)
    {
        int[] dxs = { -1, 1, 0, 0, -1, -1, 1, 1 };
        int[] dys = { 0, 0, -1, 1, -1, 1, -1, 1 };
        var options = new List<(int x, int y)>();
        for (int i = 0; i < 8; i++)
        {
            int nx = player.X + dxs[i], ny = player.Y + dys[i];
            if (map.IsWalkable(nx, ny)) options.Add((nx, ny));
        }
        if (options.Count == 0) { _log("No room to blink!"); return; }
        var (tx, ty) = options[_random.Next(options.Count)];
        map.TryMoveActor(player, tx, ty);
        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);
        _log("You blink through the void and reappear nearby.");
    }

    private void CastShadowStep(Player player, int targetX, int targetY, DungeonMap map)
    {
        var monster = map.GetMonsterAt(targetX, targetY);
        if (monster == null) { _log("No target there."); return; }

        // Try to place behind the monster (opposite side from player)
        int dx = Math.Sign(monster.X - player.X);
        int dy = Math.Sign(monster.Y - player.Y);
        int bx = monster.X + dx, by = monster.Y + dy;
        if (bx >= 0 && bx < map.Width && by >= 0 && by < map.Height && map.IsWalkable(bx, by))
        {
            map.TryMoveActor(player, bx, by);
            map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);
            _log($"You step through shadow and appear behind the {monster.Name}.");
        }
        else
        {
            // Fallback: just teleport adjacent
            CastBlink(player, map);
        }
    }

    private void CastDeathWord(Player player, int targetX, int targetY, DungeonMap map)
    {
        var monster = map.GetMonsterAt(targetX, targetY);
        if (monster == null) { _log("No target there."); return; }

        double hpPct = (double)monster.Health.CurrentHp / monster.Health.MaxHp;
        if (hpPct > 0.20)
        {
            _log($"The {monster.Name} is not weakened enough. (Must be below 20% HP)");
            return;
        }
        monster.Health.TakeDamage(monster.Health.CurrentHp);
        _log($"The Death Word unmakes the {monster.Name}.");
        CheckMonsterDeath(monster, player, map);
    }

    private void CastEldritchDrain(Player player, int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        var monster = map.GetMonsterAt(targetX, targetY);
        if (monster == null) { _log("No target there."); return; }

        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int dmg = Math.Max(1, spell.BaseDamage + intMod);
        monster.Health.TakeDamage(dmg);

        int sanityStolen = 5;
        player.Health.Heal(dmg / 2);
        player.Sanity.RestoreSanity(sanityStolen);
        _log($"You drain {dmg} life and {sanityStolen} sanity from the {monster.Name}. (+{dmg / 2} HP, +{sanityStolen} San)");
        CheckMonsterDeath(monster, player, map);
    }

    private void CastRaiseDead(Player player, int targetX, int targetY, DungeonMap map)
    {
        // Find a walkable tile near the target and "spawn" a weak undead ally
        // For now we log the effect — full ally AI would need a separate system
        _log("The dead stir... a slain creature lurches upright as your servant. (15 turns)");
        // TODO: spawn an undead ally entity when ally AI is implemented
    }

    private void CastBoneSpear(Player player, int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int dmg = Math.Max(1, spell.BaseDamage + intMod);

        // Cast along a line from player to target
        int dx = Math.Sign(targetX - player.X);
        int dy = Math.Sign(targetY - player.Y);
        int cx = player.X + dx, cy = player.Y + dy;
        int hit = 0;

        while (cx >= 0 && cx < map.Width && cy >= 0 && cy < map.Height)
        {
            var tile = map.GetTile(cx, cy);
            if (tile.Type == TileType.Wall) break;

            var monster = map.GetMonsterAt(cx, cy);
            if (monster != null)
            {
                ApplyDamageToMonster(monster, dmg, player, map);
                hit++;
                dmg = Math.Max(1, dmg * 3 / 4); // damage falls off each hit
            }
            cx += dx;
            cy += dy;
        }
        if (hit == 0) _log("The bone spear shatters against stone without striking anything.");
    }

    private void CastArmageddonRain(Player player, DungeonMap map, SpellDefinition spell)
    {
        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int killed = 0;

        foreach (var monster in map.Monsters.ToList())
        {
            int dmg = Math.Max(1, _random.Next(40, 81) + intMod);
            monster.Health.TakeDamage(dmg);
            if (monster.Health.IsDead)
            {
                killed++;
                map.Monsters.Remove(monster);
                player.Stats.Experience += monster.XpValue;
                AwardGold(monster, player);
            }
        }

        // Ignite random floor tiles
        var floors = new List<(int x, int y)>();
        for (int x = 0; x < map.Width; x++)
        for (int y = 0; y < map.Height; y++)
            if (map.GetTile(x, y).Type == TileType.Floor) floors.Add((x, y));

        int fireTiles = Math.Min(20, floors.Count / 4);
        for (int i = 0; i < fireTiles; i++)
        {
            var (fx, fy) = floors[_random.Next(floors.Count)];
            map.SetTileEffect(fx, fy, TileEffect.Fire, 8);
        }

        _log($"Meteors rain! {killed} enemies slain, {fireTiles} fires started.");
    }

    private void CastMassPetrification(DungeonMap map, SpellDefinition spell)
    {
        int count = 0;
        foreach (var monster in map.Monsters)
        {
            monster.StatusEffects.AddEffect(StatusEffectType.Frozen, spell.StatusDuration, 0);
            count++;
        }
        _log($"Stone reaches out — {count} creature(s) freeze solid for {spell.StatusDuration} turns!");
    }

    private void CastTheDreamingWord(Player player, DungeonMap map)
    {
        int driven = 0;
        foreach (var monster in map.Monsters.ToList())
        {
            int sLoss = Dice.Roll(1, 8) * 5;
            // Apply as HP damage (represents sanity collapse for monsters)
            monster.Health.TakeDamage(sLoss / 2);
            if (monster.Health.IsDead)
            {
                driven++;
                map.Monsters.Remove(monster);
                player.Stats.Experience += monster.XpValue;
                AwardGold(monster, player);
            }
        }
        _log($"The Dreaming Word reverberates. {driven} creature(s) collapse from psychic devastation!");
    }

    private void CastSummonHorde(Player player, DungeonMap map)
    {
        string[] undeadNames = { "Skeleton", "Zombie", "Ghoul" };
        int count = Dice.Roll(3, 6);
        int spawned = 0;

        for (int i = 0; i < count; i++)
        {
            string name = undeadNames[_random.Next(undeadNames.Length)];
            if (!MonsterDatabase.GetAll().TryGetValue(name, out var def)) continue;

            // Find a spawn tile near player
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int sx = player.X + _random.Next(-5, 6);
                int sy = player.Y + _random.Next(-5, 6);
                if (sx < 0 || sx >= map.Width || sy < 0 || sy >= map.Height) continue;
                if (!map.IsWalkable(sx, sy)) continue;

                var monster = new Monster
                {
                    Name = def.Name, Glyph = def.Glyph, Tier = def.Tier,
                    Damage = def.Damage, XpValue = 0, SanityDamage = 0,
                };
                monster.Health.MaxHp = def.HP;
                monster.Health.CurrentHp = def.HP;
                map.PlaceActor(monster, sx, sy);
                spawned++;
                break;
            }
        }
        _log($"A horde of {spawned} undead rises to serve you!");
    }

    private void CastRealityFracture(Player player, int targetX, int targetY, DungeonMap map)
    {
        var rng = _random;
        int radius = SpellDatabase.Get(SpellId.RealityFracture).Radius;

        // Shatter walls in radius
        int wallsOpened = 0;
        for (int dx = -radius; dx <= radius; dx++)
        for (int dy = -radius; dy <= radius; dy++)
        {
            int tx = player.X + dx, ty = player.Y + dy;
            if (tx < 1 || tx >= map.Width - 1 || ty < 1 || ty >= map.Height - 1) continue;
            if (map.GetTile(tx, ty).Type == TileType.Wall)
            {
                map.SetTile(tx, ty, TileType.Floor);
                wallsOpened++;
            }
        }

        // Spawn 1d4 random monsters in the area
        var allDefs = MonsterDatabase.GetAll().Values
            .Where(d => d.Tier >= 3 && d.Tier <= 6).ToList();
        int spawnCount = Dice.Roll(1, 4);
        for (int i = 0; i < spawnCount; i++)
        {
            var def = allDefs[rng.Next(allDefs.Count)];
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int sx = player.X + rng.Next(-radius, radius + 1);
                int sy = player.Y + rng.Next(-radius, radius + 1);
                if (sx < 0 || sx >= map.Width || sy < 0 || sy >= map.Height) continue;
                if (!map.IsWalkable(sx, sy)) continue;
                var monster = new Monster
                {
                    Name = def.Name, Glyph = def.Glyph, Tier = def.Tier,
                    Damage = def.Damage, XpValue = def.XpValue, SanityDamage = def.SanityDamage,
                    GoldMin = def.GoldMin, GoldMax = def.GoldMax,
                    GoldDropChance = def.GoldDropChance, IsEldritchCoin = def.IsEldritchCoin
                };
                monster.Health.MaxHp = def.HP;
                monster.Health.CurrentHp = def.HP;
                map.PlaceActor(monster, sx, sy);
                break;
            }
        }

        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);
        _log($"REALITY FRACTURES — {wallsOpened} walls shatter and {spawnCount} entities tear through!");
    }

    private void CastVoidCollapse(Player player, int targetX, int targetY, DungeonMap map, SpellDefinition spell)
    {
        int intMod = player.Stats.GetModifier(StatType.Intelligence);
        int radius = spell.Radius;

        // Pull all enemies toward target, deal void damage
        int pulled = 0;
        foreach (var monster in map.Monsters.ToList())
        {
            int dist = Math.Max(Math.Abs(monster.X - targetX), Math.Abs(monster.Y - targetY));
            if (dist > radius) continue;

            int voidDmg = Math.Max(1, 10 + intMod);
            ApplyDamageToMonster(monster, voidDmg, player, map);
            pulled++;
        }

        // Final explosion at target
        int explodeDmg = Math.Max(1, spell.BaseDamage + intMod);
        foreach (var monster in map.Monsters.ToList())
        {
            int dist = Math.Max(Math.Abs(monster.X - targetX), Math.Abs(monster.Y - targetY));
            if (dist <= 3)
                ApplyDamageToMonster(monster, explodeDmg, player, map);
        }

        _log($"The void singularity collapses — {pulled} enemies crushed, then annihilated by the explosion!");
    }

    private void AwardGold(Monster monster, Player player)
    {
        if (monster.GoldDropChance <= 0) return;
        if (_random.NextDouble() > monster.GoldDropChance) return;

        int gold = monster.GoldMin >= monster.GoldMax
            ? monster.GoldMin
            : _random.Next(monster.GoldMin, monster.GoldMax + 1);
        if (gold <= 0) return;

        player.Gold += gold;

        if (monster.IsEldritchCoin)
        {
            player.Sanity.LoseSanity(1);
            _log($"  You claim {gold} eldritch coins. (-1 sanity)");
        }
        else
        {
            _log($"  You find {gold} gold.");
        }
    }
}
