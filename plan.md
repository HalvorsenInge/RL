# EldritchDungeon - Complete Game Design Document

## Overview

**EldritchDungeon** is a .NET 8 ASCII roguelike combining traditional fantasy elements with Lovecraftian horror, featuring a dual resource system (HP/Mana + Sanity), extensive religion mechanics, and weapons spanning medieval through dieselpunk/lovecraftian eras.

### Core Features

- Pure ASCII console interface (80x25 standard)
- RogueSharp for map generation and pathfinding
- Ironman permadeath with JSON saves
- Dice-rolled stats (4d6 drop lowest) + racial modifiers
- Dual resource system: HP/Mana + Sanity with Lovecraftian effects
- 8 races (standard fantasy + Lovecraftian hybrids)
- 6 classes including Cultist, Gunslinger, Investigator
- 50+ weapons from medieval to dieselpunk/lovecraftian
- 40+ monsters in Lovecraftian hierarchy
- 6 gods with favor/anger mechanics and 24 powers
- Sanity healing items with addiction side effects

## Confirmed Design Decisions

| Aspect | Decision |
|--------|----------|
| **Interface** | Pure ASCII console (80x25 standard) |
| **Library** | RogueSharp for map/pathfinding |
| **Save Format** | JSON serialization |
| **.NET Version** | .NET 8 |
| **Character Creation** | Class selection menu |
| **Stats** | Dice rolled (4d6 drop lowest) + racial modifiers |
| **Sanity Healing** | Items available, with negative side effects |
| **Death** | Ironman permadeath (no saves to reload) |
| **Difficulty** | Single balanced experience |
| **Multiplayer** | Single-player only |

## Character Creation System

### Stat Rolling (4d6 drop lowest)

```
Roll 6 times (STR, DEX, CON, INT, WIS, CHA)
Minimum: 3  |  Maximum: 18
Reroll if total < 75 or any stat < 6
```

### Racial Modifiers

| Race | STR | DEX | CON | INT | WIS | CHA | HP Mod | Mana Mod | Sanity Mod |
|------|-----|-----|-----|-----|-----|-----|--------|----------|------------|
| Human | +0 | +0 | +0 | +0 | +0 | +0 | 1.0 | 1.0 | 1.0 |
| Elf | -1 | +2 | -1 | +1 | +0 | +0 | 0.9 | 1.2 | 1.0 |
| Dwarf | +2 | -1 | +2 | +0 | +0 | -1 | 1.3 | 0.8 | 1.1 |
| Halfling | -2 | +2 | +0 | +0 | +0 | +1 | 0.8 | 1.1 | 1.3 |
| Orc | +3 | -1 | +2 | -2 | -1 | -1 | 1.4 | 0.6 | 0.7 |
| DeepOneHybrid | +1 | +1 | +1 | +0 | +0 | -1 | 1.1 | 1.0 | 0.5 |
| HalfMad | +0 | +0 | -1 | +2 | -1 | +0 | 0.9 | 1.3 | 0.3 |
| SerpentFolk | +0 | +1 | +0 | +1 | +0 | +0 | 1.0 | 0.9 | 0.8 |

### Starting Equipment by Class

| Class | Weapon | Armor | Items | Gold |
|-------|--------|-------|-------|------|
| **Warrior** | Longsword | Chainmail | 3 Healing Potions | 50 |
| **Mage** | Dagger | Robe | Spellbook (Fireball), 2 Mana Potions | 30 |
| **Rogue** | Shortbow, Dagger | Leather | 15 Arrows, Lockpick Set, 2 Healing Potions | 75 |
| **Cultist** | Bone Dagger | Robe of the Deep | Chalk (Summoning), 1 Sanity Potion, Dagger | 25 |
| **Gunslinger** | Flintlock Pistol (6) | Leather Coat | 24 Bullets, Dagger, 1 Healing Potion | 40 |
| **Investigator** | Revolver, Club | Trenchcoat | 18 Bullets, Notebook, Magnifying Glass | 60 |

## Sanity System Details

### Sanity Components

```
CurrentSanity (0-100)
MaxSanity = 100 + (WIS modifier × 10)
SanityThresholdMinor = 50 (unsettling effects below)
SanityThresholdMajor = 25 (severe effects below)
InsanityResist = 0.0-0.5 (modified by race/class/gods)
```

### Sanity States

| State | Range | Effects |
|-------|-------|---------|
| **Stable** | 100-51 | Normal gameplay |
| **Fractured** | 50-26 | Unreliable messages, minor hallucinations |
| **Unraveling** | 25-10 | Fake monsters appear, stat fluctuations |
| **Broken** | 9-0 | Random actions, cannot cast, risk of heart attack |

### Sanity Damage Sources

| Source | Amount | Notes |
|--------|--------|-------|
| Deep One | 5-15 | Aquatic horror |
| Mi-Go | 10-20 | Brain harvesting |
| Shoggoth | 25-40 | Organic nightmare |
| Elder Thing | 15-25 | Mechanical alien |
| Star-Spawn | 30-50 | Divine horror |
| Great Old One | 100 | Instant breakdown/death |
| Forbidden Tome | 10-30 | While reading |
| Sanity Potion (alcoholic) | +30 | Healing with addiction risk |

### Sanity Healing Items (With Drawbacks)

| Item | Heal | Side Effect | Severity |
|------|------|-------------|----------|
| **Sanctified Water** | +30 | None | Safe |
| **Elixir of Calm** | +25 | -5 WIS for 100 turns | Mild |
| **Mindcrust** | +50 | -10 INT for 200 turns, addiction | Severe |
| **Deep One Whiskey** | +40 | Drunk (accuracy -20%), addiction, -5 CON | Severe |
| **Cultist Opium** | +60 | Hallucinations, -15 all stats for 300 turns, addiction | Critical |
| **Void Essence** | +100 | Reality tears (random teleportation), -20 all stats | Catastrophic |

### Addiction System

```
AddictionLevel (0-100)
Threshold: 50 = Addicted

Effects of Addiction:
- Craving when not using (messages, -5 to all checks)
- Withdrawal at addiction >50 (-10 to all stats)
- Tolerance increases (need more for same effect)
- Cannot cure without "Cold Turkey" (leave dungeon)
```

### Hallucination Events (Fractured/Unraveling)

```
[SANITY EVENT] You hear footsteps behind you...
[SANITY EVENT] Is that a goblin, or just a shadow? (fake monster)
[SANITY EVENT] The walls are breathing...
[SANITY EVENT] Was that message real, or your imagination?
[SANITY EVENT] You recognize this place... but you've never been here.
```

## Religion System Details

### Gods

| God | Domain | Favor Bonus | Anger Trigger | Tier 1 Power | Tier 2 Power | Tier 3 Power | Tier 4 Power |
|-----|--------|-------------|---------------|--------------|--------------|--------------|--------------|
| **Cthulhu** | Dreams | HP Regen, Night Vision | Attack Deep Ones | Dream Walk | Tentacle Slam | Cult Summon | R'lyeh Rising |
| **Nyarlathotep** | Chaos | Crit Chance, Loot | Betrayal, breaking promises | Mimic | Teleport | Corrupt | Walk Between |
| **Azathoth** | Void | Mana, Spell Power | Use holy magic | Void Shield | Void Blast | Nullify | Reality Bend |
| **Yog-Sothoth** | Knowledge | XP Gain, Identify | Attack scholars | Clairvoyance | Scry | The Word | Omnipresence |
| **Hastur** | Stars | Sanity Resist, Range | Look at stars wrong | Starlight | Stars Emit | Tentacles | Yellow Sign |
| **Dagon** | Deep | Water Abilities, Damage | Desecrate water | Water Breath | Flood | Tsunami | Abyssal Form |

### Favor Mechanics

```
Favor: 0-100
Anger: 0-100

Favor Gain:
- Sacrifice appropriate item: +5-15
- Complete quest: +25
- Daily prayer: +2
- Kill enemy of god: +3

Favor Loss:
- Attack priest: -30
- Use anti-god item: -20
- Wrong sacrifice: -10
- Abandon faith: -50

Anger Gain:
- Kill worshipper: +25
- Defile altar: +40
- Switch gods: +50

Anger Reduction:
- Apology prayer: -5 (while <50 anger)
- Major sacrifice: -20
```

### God Powers (by Tier)

**Cthulhu**
```
Tier 1 (Favor 25): Dreams - Regenerate HP while sleeping (2 HP/turn)
Tier 2 (Favor 50): Tentacle Slam - 30 void damage, stuns enemy
Tier 3 (Favor 75): Cult Summon - Summon 2d4 cultists (friendly)
Tier 4 (Favor 100): R'lyeh Rising - Earthquake, summons 10 undead
```

**Nyarlathotep**
```
Tier 1 (Favor 25): Lucky Coin - Reroll one dice roll
Tier 2 (Favor 50): Mimicry - Transform into monster for 20 turns
Tier 3 (Favor 75): Corrupt - Convert enemy to follower (non-boss)
Tier 4 (Favor 100): Walk Between - Teleport anywhere on map
```

**Azathoth**
```
Tier 1 (Favor 25): Mana Well - 1 MP/turn regen
Tier 2 (Favor 50): Void Blast - Pure void damage (ignores armor)
Tier 3 (Favor 75): Nullify - Remove all magic from target
Tier 4 (Favor 100): Reality Bend - Reroll entire room layout
```

**Yog-Sothoth**
```
Tier 1 (Favor 25): Ancient Knowledge - Auto-identify item
Tier 2 (Favor 50): Clairvoyance - Reveal entire floor map
Tier 3 (Favor 75): The Word - Kill any enemy (non-boss)
Tier 4 (Favor 100): Omnipresence - Act twice per turn
```

**Hastur**
```
Tier 1 (Favor 25): Starlight - Light up room, +10% accuracy
Tier 2 (Favor 50): Stars Emit - AoE star damage, -10 sanity
Tier 3 (Favor 75): Yellow Sign - Enemies flee in terror
Tier 4 (Favor 100): The King in Yellow - Summon Hastur's avatar
```

**Dagon**
```
Tier 1 (Favor 25): Water Breath - Breathe underwater
Tier 2 (Favor 50): Flood - Fill room with shallow water
Tier 3 (Favor 75): Tsunami - Massive water damage, push enemies
Tier 4 (Favor 100): Abyssal Form - Transform into Deep One, +100 HP, +20 damage
```

## Weapon Database (Complete)

### Medieval Weapons (Tiers 1-3)

| Weapon | Damage | Crit | Speed | Range | Special |
|--------|--------|------|-------|-------|---------|
| Dagger | 4 | 18-20×2 | Fast | Melee | Backstab +50% |
| Shortsword | 8 | 19-20×2 | Normal | Melee | - |
| Longsword | 12 | 19-20×2 | Normal | Melee | Parry +1 |
| Battleaxe | 15 | 20×3 | Slow | Melee | Armor pierce 5 |
| Mace | 10 | 20×2 | Normal | Melee | Ignore armor |
| Spear | 10 | 20×2 | Normal | Reach (2) | Set against charge |
| Warhammer | 14 | 20×3 | Slow | Melee | Concussive |
| Halberd | 14 | 19-20×2 | Slow | Reach (2) | Hook |
| Longbow | 8 | 20×3 | Normal | 10 | Volley |
| Crossbow | 12 | 19-20×2 | Slow | 15 | Precise |

### Early Modern Weapons (Tiers 3-5)

| Weapon | Damage | Crit | Range | Ammo | Special |
|--------|--------|------|-------|------|---------|
| Flintlock Pistol | 20 | 20×2 | 8 | 6 | Misfire 10% |
| Blunderbuss | 25 | 20×2 | 4 | 4 | Spread (AoE) |
| Musket | 22 | 20×2 | 12 | 1 | High damage |
| Rapier | 10 | 18-20×3 | Melee | - | Very fast |
| Bayonet | 14 | 20×2 | Reach (1) | - | Attached to musket |
| Grenade | 30 | - | 5 | 3 | AoE, self-damage |
| Pepperbox | 18 | 20×2 | 8 | 6 | Fast reload |
| Sword-Cane | 8 | 20×2 | Melee | - | Hidden, +5 CHA |

### Dieselpunk Weapons (Tiers 5-7)

| Weapon | Damage | Crit | Range | Ammo | Special |
|--------|--------|------|-------|------|---------|
| Clockwork Pistol | 28 | 20×3 | 10 | 8 | No misfire |
| Steam-Cannon | 45 | 20×2 | 6 | 2 | Slow, stationary |
| Alchemical Bomb | 50 | - | 6 | 3 | Elemental AoE |
| Voltaic Staff | 25 | 20×2 | 4 | - | 5 MP/shock |
| Gyrojet Rifle | 35 | 20×3 | 15 | 5 | Explosive rounds |
| Automaton Fist | 40 | 20×2 | Melee | - | Arm-mounted |
| Chemthrower | 30 | - | 6 | 4 | Burning damage |
| Thunder Hammer | 35 | 20×3 | 2 | - | Stun chance |

### Lovecraftian Weapons (Tiers 7-10)

| Weapon | Damage | Crit | Range | Special |
|--------|--------|------|-------|---------|
| Bone Dagger (Deep One) | 20 | 20×3 | Melee | +15 sanity drain |
| Star-Metal Blade | 35 | 19-20×3 | Melee | Ignores armor |
| Dream Whip | 15 | 20×4 | 3 | +50 sanity damage |
| Shoggoth Slime | 30 | 20×2 | Melee | Acid (5 dmg/turn) |
| Forbidden Tome | 10 | 20×2 | Melee | Cast spells on hit |
| Ichor Spear | 28 | 20×2 | Reach (2) | Poison (10 dmg/turn) |
| Star-Visor | - | - | - | See invisible, +20 sanity dmg |
| Heart of Cthulhu | 100 | 20×5 | Melee | Legend tier |

## Monster Database (Complete)

### Tier 1-2 (Dungeon Levels 1-3)

| Monster | HP | Damage | Sanity | XP | Abilities |
|---------|-----|--------|--------|-----|-----------|
| Rat | 5 | 2 | 0 | 10 | Swarm (5+) |
| Giant Bat | 8 | 3 | 0 | 15 | Flying |
| Goblin | 15 | 5 | 0 | 25 | Numbers |
| Kobold | 12 | 4 | 0 | 20 | Traps |
|Skeleton| 20 | 8 | 2 | 35 | Undead |
| Zombie | 25 | 6 | 1 | 30 | Slow, rot |
| Giant Spider | 18 | 5 | 3 | 40 | Poison |
| Cave Spider | 12 | 3 | 2 | 30 | Web |

### Tier 3-4 (Dungeon Levels 4-7)

| Monster | HP | Damage | Sanity | XP | Abilities |
|---------|-----|--------|--------|-----|-----------|
| Orc | 35 | 12 | 0 | 75 | Berserk |
| Dark Elf | 30 | 14 | 2 | 80 | Magic (-10 HP) |
| Ghoul | 40 | 10 | 5 | 90 | Paralysis |
| Ogre | 60 | 18 | 3 | 100 | Throw rocks |
| Dark Cultist | 35 | 10 | 8 | 100 | Summon |
| Worm | 25 | 15 | 4 | 85 | Burrow |
| Fire Vampire | 25 | 15 | 5 | 95 | Life drain |
| Shadow | 20 | 8 | 10 | 110 | Invisibility |

### Tier 5-6 (Dungeon Levels 8-12)

| Monster | HP | Damage | Sanity | XP | Abilities |
|---------|-----|--------|--------|-----|-----------|
| Deep One | 50 | 18 | 15 | 200 | Amphibious, Devolve |
| Mi-Go | 55 | 20 | 20 | 220 | Flying, Brain harvest |
| Ghast | 45 | 22 | 12 | 200 | Stench, Rot |
| Wight | 60 | 15 | 15 | 210 | Level drain |
| Phantom | 35 | 12 | 25 | 230 | Possession |
| Byakhee | 50 | 18 | 10 | 200 | Flying, Poison |
| Hunting Horror | 40 | 25 | 30 | 250 | Constrict |
| Star-Spawn (Lesser) | 70 | 25 | 20 | 250 | Mythos magic |

### Tier 7-8 (Dungeon Levels 13-18)

| Monster | HP | Damage | Sanity | XP | Abilities |
|---------|-----|--------|--------|-----|-----------|
| Elder Thing | 80 | 30 | 25 | 400 | Sonic attack |
| Shoggeth | 100 | 35 | 35 | 450 | Regeneration, Acid |
| High Cultist | 70 | 25 | 30 | 380 | Summon, Sanity blast |
| Chthonian | 90 | 28 | 20 | 400 | Burrow, Ambush |
| Fire Elemental | 70 | 35 | 5 | 350 | Fire immunity |
| Vampire Lord | 120 | 30 | 25 | 450 | Domination |
| Flying Polyp | 80 | 25 | 40 | 480 | Telekinesis |
| Serpent of N'kai | 95 | 32 | 35 | 500 | Hypnosis |

### Tier 9-10 (Dungeon Levels 19-25)

| Monster | HP | Damage | Sanity | XP | Abilities |
|---------|-----|--------|--------|-----|-----------|
| High Priest | 150 | 45 | 50 | 1000 | Summon, Miracles |
| Star-Spawn (Greater) | 180 | 55 | 60 | 1200 | Reality warp |
| Dagon's Avatar | 200 | 50 | 55 | 1300 | Water control |
| Hastur's Avatar | 220 | 60 | 70 | 1400 | Yellow Sign |
| Nyarlathotep's Avatar| 250 | 65 | 75 | 1500 | Chaos magic |
| Shuggoth (Elder) | 300 | 70 | 80 | 1800 | Hive mind |
| Great Old One | 500 | 100 | 100 | 5000 | Reality destruction |
| Cthulhu | 666 | 150 | 200 | 10000 | Dream revival |

## Technical Architecture

### Core Enums and Types

```csharp
public enum SanityState { Stable, Edge, Fractured, Unraveling, Broken }
public enum DamageType { Physical, Mana, Sanity, Holy, Void }
public enum WeaponCategory { 
    Medieval,     // Sword, Axe, Mace, Spear, Dagger, Longbow, Crossbow
    EarlyModern,  // Flintlock, Blunderbuss, Bayonet, Grenade
    Dieselpunk,   // Clockwork, Steam, Alchemical, Voltaic
    Lovecraftian  // Bone, Star-metal, Dream-weapons
}
public enum RaceType { Human, Elf, Dwarf, Halfling, Orc, DeepOneHybrid, HalfMad, SerpentFolk }
public enum ClassType { Warrior, Mage, Rogue, Cultist, Gunslinger, Investigator }
public enum GodType { Cthulhu, Nyarlathotep, Azathoth, YogSothoth, Hastur, Dagon }
```

### Component-Based Architecture

All actors use components for modularity:
- `HealthComponent` - HP management
- `ManaComponent` - Mana management  
- `SanityComponent` - Sanity with thresholds and resistance
- `InventoryComponent` - Item storage and management
- `EquipmentComponent` - Worn/wielded items
- `StatsComponent` - RPG attributes
- `StatusEffectsComponent` - Buffs/debuffs
- `ReligionComponent` - God favor and powers

### Map System (RogueSharp Integration)

```csharp
public class DungeonMap : Map
{
    public List<Room> Rooms { get; set; }
    public List<Stairs> Stairs { get; set; }
    
    public void Generate(int level, int width, int height)
    {
        // BSP room generation
        var generator = new ProceduralGenerator();
        generator.GenerateDungeon(this, level);
        
        // Place entities based on level
        var placer = new EntityPlacer();
        placer.PlaceMonsters(this, level);
        placer.PlaceItems(this, level);
    }
}
```

## Final Folder Structure

```
EldritchDungeon/
├── src/
│   ├── EldritchDungeon.Core/
│   │   ├── GameConstants.cs
│   │   ├── Enums.cs
│   │   ├── GameConfig.cs
│   │   └── Dice.cs
│   │
│   ├── EldritchDungeon.Entities/
│   │   ├── Actor.cs
│   │   ├── Player.cs
│   │   ├── Monster.cs
│   │   ├── NPC.cs
│   │   │
│   │   ├── Components/
│   │   │   ├── Component.cs
│   │   │   ├── HealthComponent.cs
│   │   │   ├── ManaComponent.cs
│   │   │   ├── SanityComponent.cs
│   │   │   ├── InventoryComponent.cs
│   │   │   ├── EquipmentComponent.cs
│   │   │   ├── StatsComponent.cs
│   │   │   ├── StatusEffectsComponent.cs
│   │   │   └── ReligionComponent.cs
│   │   │
│   │   └── Items/
│   │       ├── Item.cs
│   │       ├── Weapon.cs
│   │       ├── Armor.cs
│   │       ├── Consumable.cs
│   │       ├── Scroll.cs
│   │       ├── Artifact.cs
│   │       └── ItemQuality.cs
│   │
│   ├── EldritchDungeon.Systems/
│   │   ├── System.cs
│   │   ├── CombatSystem.cs
│   │   ├── SanitySystem.cs
│   │   ├── MagicSystem.cs
│   │   ├── ReligionSystem.cs
│   │   ├── AISystem.cs
│   │   ├── StatusEffectSystem.cs
│   │   ├── LevelingSystem.cs
│   │   ├── SanityEventSystem.cs
│   │   └── AddictionSystem.cs
│   │
│   ├── EldritchDungeon.World/
│   │   ├── Map/
│   │   │   ├── DungeonMap.cs
│   │   │   ├── Tile.cs
│   │   │   ├── Room.cs
│   │   │   └── MapGenerator.cs
│   │   ├── Level.cs
│   │   └── Dungeon.cs
│   │
│   ├── EldritchDungeon.Data/
│   │   ├── Races/
│   │   │   ├── RaceDefinition.cs
│   │   │   └── RaceDatabase.cs
│   │   ├── Classes/
│   │   │   ├── ClassDefinition.cs
│   │   │   └── ClassDatabase.cs
│   │   ├── Monsters/
│   │   │   ├── MonsterDefinition.cs
│   │   │   └── MonsterDatabase.cs
│   │   ├── Items/
│   │   │   ├── WeaponDatabase.cs
│   │   │   ├── ArmorDatabase.cs
│   │   │   └── ItemDatabase.cs
│   │   ├── Spells/
│   │   │   ├── SpellDefinition.cs
│   │   │   └── SpellDatabase.cs
│   │   ├── Gods/
│   │   │   ├── GodDefinition.cs
│   │   │   └── GodDatabase.cs
│   │   └── SanityEvents/
│   │       └── SanityEventDatabase.cs
│   │
│   ├── EldritchDungeon.UI/
│   │   ├── ASCIIRenderer.cs
│   │   ├── MessageLog.cs
│   │   ├── Screen.cs
│   │   ├── GameScreen.cs
│   │   ├── CharacterScreen.cs
│   │   ├── InventoryScreen.cs
│   │   ├── SpellScreen.cs
│   │   ├── ReligionScreen.cs
│   │   ├── MessageScreen.cs
│   │   ├── MenuScreen.cs
│   │   └── HelpScreen.cs
│   │
│   ├── EldritchDungeon.Engine/
│   │   ├── GameEngine.cs
│   │   ├── GameLoop.cs
│   │   ├── InputHandler.cs
│   │   ├── TurnManager.cs
│   │   └── SaveManager.cs
│   │
│   ├── EldritchDungeon.Generation/
│   │   ├── ProceduralGenerator.cs
│   │   ├── RoomGenerator.cs
│   │   ├── LootGenerator.cs
│   │   ├── MonsterPlacer.cs
│   │   ├── TrapGenerator.cs
│   │   └── SpecialRoomGenerator.cs
│   │
│   └── EldritchDungeon.Utilities/
│       ├── ColorPalette.cs
│       ├── Glyphs.cs
│       └── MathUtils.cs
│
├── tests/
│   ├── EldritchDungeon.Core.Tests/
│   ├── EldritchDungeon.Systems.Tests/
│   └── EldritchDungeon.Data.Tests/
│
├── docs/
│   ├── DESIGN.md
│   ├── COMBAT_SYSTEM.md
│   ├── SANITY_SYSTEM.md
│   ├── RELIGION_SYSTEM.md
│   └── ITEM_DATABASE.md
│
├── content/
│   ├── saves/
│   └── screenshots/
│
├── EldritchDungeon.sln
└── README.md
```

## Implementation Phases Summary

| Phase | Duration | Deliverables |
|-------|----------|--------------|
| **Phase 1** | Days 1-3 | Project setup, core types, enums, player with components |
| **Phase 2** | Days 4-7 | Data systems (races, classes, monsters, items, gods) |
| **Phase 3** | Days 8-12 | Map generation (RogueSharp), rendering, basic AI |
| **Phase 4** | Days 13-16 | Combat, sanity, religion systems |
| **Phase 5** | Days 17-20 | UI screens, message log |
| **Phase 6** | Days 21-24 | Save/load, content expansion |
| **Phase 7** | Days 25-30 | Balance, polish, special rooms, sanity events |

## Key Technical Considerations

### RogueSharp Usage
- `Map` class for tile data
- `Cell` for individual tiles
- `Pathfinder` for monster AI
- `FieldOfView` for visibility calculations
- `IMapCreationStrategy` for procedural generation

### Sanity System Integration
- Hook into game loop after every action
- Check for Lovecraftian monster proximity
- Random events trigger at thresholds
- Render fake monsters in FOV when hallucinating

### Console Performance
- Use `Console.SetCursorPosition` for updates
- Double-buffer for smooth rendering
- Only redraw changed cells

### Extensibility
- Component-based architecture for easy additions
- JSON data files for new monsters/items/gods
- Event system for game events

## Victory Conditions

- Primary: Defeat Cthulhu (Level 25 final boss)
- Secondary: Become a god's avatar (Favor 100 with any god)
- Tertiary: Collect all legendary weapons
- Ironman: Complete game without dying once

## Multiple Endings

- **Victory**: Defeat Cthulhu and save humanity
- **Godhood**: Become Cthulhu's avatar
- **Madness**: Break down completely but survive
- **Doom**: Reality destroyed by awakened Great Old One
- **Escape**: Flee dungeon as survivor (partial victory)

---

## Ready to Implement

All design decisions are confirmed. The plan covers:

✅ Pure ASCII console interface with RogueSharp
✅ Ironman permadeath
✅ Dice-rolled stats with racial modifiers  
✅ Class selection menu with starting equipment
✅ Dual HP/Mana + Sanity resource system
✅ Sanity healing items with addiction drawbacks
✅ 8 races (standard + Lovecraftian)
✅ 6 classes (including Cultist, Gunslinger, Investigator)
✅ 50+ weapons (medieval through dieselpunk/lovecraftian)
✅ 40+ monsters (Lovecraftian horror hierarchy)
✅ 6 gods with favor/anger mechanics and 24 powers
✅ JSON save system
✅ Component-based architecture for extensibility