# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EldritchDungeon** — a .NET 8 ASCII roguelike combining traditional fantasy with Lovecraftian horror. Pure console interface (80x25), ironman permadeath, JSON saves. Not yet implemented; `plan.md` is the authoritative design document.

## Planned Tech Stack

- **Language/Runtime**: C# / .NET 8
- **Key Dependency**: RogueSharp (map generation, pathfinding, FOV)
- **UI**: Pure ASCII console with double-buffered rendering
- **Save System**: JSON serialization

## Build & Run Commands

**Important:** `dotnet` is not on the default PATH in this WSL environment. Do NOT attempt to run `dotnet build`, `dotnet test`, or `dotnet run` — they will fail with "command not found". The user builds and tests outside of Claude Code.

## Architecture

**Component-based entity system** — all actors (Player, Monster, NPC) extend `Actor` and compose behavior through components: `HealthComponent`, `ManaComponent`, `SanityComponent`, `InventoryComponent`, `EquipmentComponent`, `StatsComponent`, `StatusEffectsComponent`, `ReligionComponent`.

**Game systems** operate on components: `CombatSystem`, `SanitySystem`, `MagicSystem`, `ReligionSystem`, `AISystem`, `StatusEffectSystem`, `LevelingSystem`, `SanityEventSystem`, `AddictionSystem`.

**Data layer** uses static databases (`RaceDatabase`, `ClassDatabase`, `MonsterDatabase`, `WeaponDatabase`, `GodDatabase`, etc.) that define all game content as data.

### Key Domain Concepts

- **Dual resource system**: HP/Mana (standard) + Sanity (0-100, with states: Stable > Fractured > Unraveling > Broken)
- **Religion**: 6 gods with favor (0-100) and anger (0-100) mechanics; 4-tier powers per god
- **Addiction**: Sanity-healing items carry addiction risk (threshold 50 = addicted, withdrawal effects)
- **Weapons span 4 eras**: Medieval → Early Modern → Dieselpunk → Lovecraftian
- **Monsters in 5 tiers**: dungeon depth determines encounter tier (1-2 through 9-10)

### Project Structure (planned)

```
src/
  EldritchDungeon.Core/        # Constants, enums, dice, config
  EldritchDungeon.Entities/    # Actor, Player, Monster, Components/, Items/
  EldritchDungeon.Systems/     # Game systems (combat, sanity, religion, AI, etc.)
  EldritchDungeon.World/       # DungeonMap, Tile, Room, MapGenerator
  EldritchDungeon.Data/        # Static databases for races, classes, monsters, items, gods
  EldritchDungeon.UI/          # ASCIIRenderer, screens (Game, Character, Inventory, etc.)
  EldritchDungeon.Engine/      # GameEngine, GameLoop, InputHandler, TurnManager, SaveManager
  EldritchDungeon.Generation/  # Procedural dungeon, loot, monster placement
  EldritchDungeon.Utilities/   # Color palette, glyphs, math helpers
tests/
  EldritchDungeon.Core.Tests/
  EldritchDungeon.Systems.Tests/
  EldritchDungeon.Data.Tests/
```

## Spell System

Spells are defined in `src/EldritchDungeon.Data/Spells/SpellDatabase.cs` and cast by `MagicSystem` in Systems. Key facts:

- `Player.KnownSpells` is a `List<SpellId>` in discovery order — persisted to save files.
- **Mage** starts with Fireball, Magic Bolt, Mage Armor. **Cultist** starts with Void Bolt, Magic Bolt.
- Key `m` opens the Spellbook. Targeted spells use the same cursor system as ranged weapons.
- Broken sanity (0-9) prevents spellcasting.

### Tile Effects (terrain interactions)

`Tile.Effect` (TileEffect enum) + `Tile.EffectDuration` track environmental hazards:

| Effect | Glyph | Notes |
|--------|-------|-------|
| Water  | `~`   | Permanent. Conducts lightning. Extinguished by cold. |
| Fire   | `^`   | 8–10 turns. 5 dmg/turn to actors on it. |
| Steam  | `*`   | 5 turns. Blocks FOV/LOS. |
| Oil    | `%`   | Permanent. Fire/lightning ignites it (explosion). |

**Interaction matrix**: Fire+Water→Steam; Fire+Oil→Explosion; Lightning+Water→Electric shock (flood-fill); Lightning+Oil→Fire; Cold+Fire→Extinguish.

### Super Spells

Super spells (`IsSuperSpell = true` in SpellDefinition) are high-cost, dramatic level-wide effects. They appear distinctly in the Spellbook UI with `[SUPER]` tag. New super spells are added to `SpellId` enum and `SpellDatabase`, and handled as special cases in `MagicSystem.CastSpell`.

**Current super spells:**
- `EyeInTheSky` (50 MP) — Reveals every monster (name, HP, position) and item on the level. Itemized report added to the message log; log opens automatically after casting.

When the user announces a new super spell, add it to `SpellId`, `SpellDatabase`, and `MagicSystem`. Handle the display result (screen or log) consistently with existing super spells.

## Design Reference

All game data (racial modifiers, class starting equipment, weapon stats, monster stats, god powers, sanity thresholds) is fully specified in `plan.md`. Consult it before implementing any game content — do not invent stats or mechanics.

## Console Rendering Notes

- Use `Console.SetCursorPosition` for targeted updates
- Double-buffer to avoid flicker
- Only redraw changed cells for performance
- Standard terminal: 80x25 characters
