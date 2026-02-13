# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EldritchDungeon** — a .NET 8 ASCII roguelike combining traditional fantasy with Lovecraftian horror. Pure console interface (80x25), ironman permadeath, JSON saves. Not yet implemented; `plan.md` is the authoritative design document.

## Planned Tech Stack

- **Language/Runtime**: C# / .NET 8
- **Key Dependency**: RogueSharp (map generation, pathfinding, FOV)
- **UI**: Pure ASCII console with double-buffered rendering
- **Save System**: JSON serialization

## Build & Run Commands (once scaffolded)

```bash
dotnet build EldritchDungeon.sln
dotnet run --project src/EldritchDungeon.Engine
dotnet test                                    # all tests
dotnet test tests/EldritchDungeon.Core.Tests   # single test project
```

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

## Design Reference

All game data (racial modifiers, class starting equipment, weapon stats, monster stats, god powers, sanity thresholds) is fully specified in `plan.md`. Consult it before implementing any game content — do not invent stats or mechanics.

## Console Rendering Notes

- Use `Console.SetCursorPosition` for targeted updates
- Double-buffer to avoid flicker
- Only redraw changed cells for performance
- Standard terminal: 80x25 characters
