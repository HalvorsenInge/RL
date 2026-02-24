# EldritchDungeon - Complete Game Design Document

## Overview

**EldritchDungeon** is a .NET 8 ASCII roguelike combining traditional fantasy elements with Lovecraftian horror, featuring a dual resource system (HP/Mana + Sanity), extensive religion mechanics, and weapons spanning medieval through dieselpunk/lovecraftian eras.

### Core Features

- Pure ASCII console interface (80x25 screen, scrolling camera over maps up to 140x70)
- RogueSharp for map generation and pathfinding
- Ironman permadeath with JSON saves
- Dice-rolled stats (4d6 drop lowest) + racial modifiers
- Dual resource system: HP/Mana + Sanity with Lovecraftian effects
- 8 races (standard fantasy + Lovecraftian hybrids)
- 6 classes including Cultist, Gunslinger, Investigator
- 50+ weapons from medieval to dieselpunk/lovecraftian
- 40+ monsters in Lovecraftian hierarchy
- 6 gods that observe and react to player actions — no worship required
- Sanity healing items with addiction side effects

## Confirmed Design Decisions

| Aspect | Decision |
|--------|----------|
| **Interface** | Pure ASCII console, 80x25 screen; maps up to 140x70 with scrolling camera |
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

### Design Philosophy

Gods are cosmic observers — they do not require worship. They simply watch, and react. The player does not "follow" a god; gods respond to actions whether or not the player cares. High Favor means a god has taken a liking to you. High Anger means they want you dead. Both can happen simultaneously.

There are no prayers, sacrifices, or daily rituals. You earn (or lose) divine regard through what you do.

### God Profiles

Each god has a personality expressed through what they like and hate, which monsters are under their protection, and what monsters they despise. These are **semi-randomized per run**: each playthrough seeds slight variations (e.g., Cthulhu might hate Skeletons this run, or Deep Ones, depending on RNG). Core traits remain stable; only 1-2 preferences are shuffled.

| God | Domain | Personality |
|-----|--------|-------------|
| **Cthulhu** | Dreams, Deep | Patient, proprietary — deeply jealous of his servants. Rewards those who spread his influence. |
| **Nyarlathotep** | Chaos, Messenger | Capricious, amused by suffering and irony. Loves chaos, hates order and stagnation. |
| **Azathoth** | Void, Entropy | Mindless but reactive — patterns of violence and magic ripple through him unpredictably. |
| **Yog-Sothoth** | Knowledge, Gates | Cerebral and detached. Appreciates learning and exploration; despises ignorance and wanton destruction. |
| **Hastur** | Stars, Madness | Vain and theatrical. Loves spectacle and suffering; hates being ignored or mocked. |
| **Dagon** | Sea, Deep Ones | Territorial and primal. Fiercely protects his chosen creatures; loathes fire and holy magic. |

### Favor & Anger Mechanics

```
Favor: 0-100   (divine regard — unlocks passive blessings and powers)
Anger: 0-100   (divine wrath — triggers curses, interventions, and monster summons)

Both values are INDEPENDENT. You can have Favor 80 and Anger 60 simultaneously.
```

**Favor rises when you do things a god loves.** Examples:
- Kill monsters the god hates
- Destroy altars belonging to a rival god
- Use the god's favored damage type
- Find and leave alone monsters the god protects (they notice)

**Favor falls when you do things a god hates.** Examples:
- Kill monsters under the god's protection
- Desecrate their altars
- Use damage types they despise
- Pick up or destroy items sacred to them

**Anger rises from affronts:**
- Desecrating their altar: +40
- Slaying their champion or avatar: +50
- Killing many of their favored monsters quickly: +5 per kill above threshold
- Attacking with a weapon anathema to them: +10

**Anger fades slowly over time** (1 point per ~20 turns) unless you keep committing affronts. There is no prayer or apology — actions speak, not words.

### Per-God Preferences (Base — partially randomized per run)

**Cthulhu**
```
Loved monsters:    Deep One, Star-Spawn (both tiers), Dark Cultist
Hated monsters:    [1 random from: Skeleton, Ghoul, Vampire Lord, High Priest]
Loved actions:     Kill undead, enter water tiles, go deeper into dungeon
Hated actions:     Use holy magic, attack Deep Ones, destroy water sources
Favored damage:    Void, Sanity
Anathema damage:   Holy
God summons (anger ≥60): Sends 1d4 Deep Ones + 1 Dark Cultist
God summons (anger ≥90): Sends Star-Spawn (Lesser) + wave of Deep Ones
```

**Nyarlathotep**
```
Loved monsters:    Shadow, Fire Vampire, Dark Elf
Hated monsters:    [1 random from: Goblin, Kobold, Orc, Zombie]
Loved actions:     Open locked chests, kill unique/named enemies, trigger traps on enemies
Hated actions:     Leave a floor without killing anything, heal to full HP twice in a row
Favored damage:    Fire, Chaos (any DoT)
Anathema damage:   Cold
God summons (anger ≥60): Sends 2d3 Shadows + 1 Fire Vampire
God summons (anger ≥90): Sends Nyarlathotep's Avatar
```

**Azathoth**
```
Loved monsters:    Flying Polyp, Shoggoth variants, Elder Thing
Hated monsters:    [1 random from: Ghoul, Rat, Giant Bat, Kobold]
Loved actions:     Cast spells, destroy walls/terrain, trigger explosions
Hated actions:     Use non-magical weapons exclusively for 10+ turns, enter temples
Favored damage:    Void, Explosion
Anathema damage:   Physical (mundane steel)
God summons (anger ≥60): Sends Flying Polyp + 1d3 Shoggoths
God summons (anger ≥90): Reality tear — spawns random tier 7-8 monster at player's feet
```

**Yog-Sothoth**
```
Loved monsters:    Phantom, Wight, Serpent of N'kai
Hated monsters:    [1 random from: Goblin, Kobold, Rat, Zombie]
Loved actions:     Identify items, read tomes/scrolls, discover new dungeon rooms
Hated actions:     Destroy books/scrolls, skip floors without exploring ≥70% of tiles
Favored damage:    Sanity, Magic
Anathema damage:   Fire (destroys knowledge)
God summons (anger ≥60): Sends 2 Phantoms + 1 Wight
God summons (anger ≥90): Sends Serpent of N'kai + temporal echo (duplicate of a recently killed enemy)
```

**Hastur**
```
Loved monsters:    Byakhee, Hunting Horror, High Cultist
Hated monsters:    [1 random from: Skeleton, Zombie, Rat, Ghoul]
Loved actions:     Take sanity damage (ironically pleasing), kill enemies in one hit, operate in darkness
Hated actions:     Use light sources aggressively, heal sanity, destroy star-pattern tiles
Favored damage:    Sanity, Cold
Anathema damage:   Holy, Light
God summons (anger ≥60): Sends 1d4 Byakhee + Hunting Horror
God summons (anger ≥90): Yellow Sign curse — all enemies in FOV become frenzied; Hastur's Avatar spawns at stairs
```

**Dagon**
```
Loved monsters:    Deep One, Chthonian, Giant Spider (aquatic variants)
Hated monsters:    [1 random from: Fire Elemental, Fire Vampire, Orc, Dark Elf]
Loved actions:     Move through water tiles, kill fire monsters, go deeper
Hated actions:     Use fire magic, drain/destroy water tiles, kill Deep Ones
Favored damage:    Cold, Poison, Physical (crushing)
Anathema damage:   Fire, Lightning (in water)
God summons (anger ≥60): Sends 1d6 Deep Ones
God summons (anger ≥90): Dagon's Avatar emerges from nearest water tile (or spawns mid-room if none)
```

### Divine Interventions (Favor-based)

Gods occasionally act in your favor when Favor is high — unprompted, unrequested. These are **not powers you activate**; they just happen.

| Favor Range | Example Intervention |
|-------------|---------------------|
| 25-49 | Brief blessing: +5 to next attack roll; a killed monster drops extra loot |
| 50-74 | Passive aura: matching damage type +10% for current floor |
| 75-89 | Direct gift: a god-specific item appears at your feet |
| 90-100 | Champion status: god summons allies to fight alongside you (1d4 creatures they love) |

### God Powers (Favor-Gated, Passive Unlocks)

These unlock automatically at Favor thresholds — no activation required unless noted.

**Cthulhu**
```
Favor 25:  Dreamer's Resilience — regenerate 1 HP/turn in dark rooms
Favor 50:  Void Sight — see in darkness; Deep Ones ignore you unless attacked
Favor 75:  Tentacle Lash — on melee kill, chance to proc tentacle for +20 void damage
Favor 100: R'lyeh Rises — active; earthquake collapses walls, spawns 2d6 undead allies
```

**Nyarlathotep**
```
Favor 25:  Lucky Devil — once per floor, reroll a missed attack
Favor 50:  Shifting Face — once per floor, transform appearance (enemies ignore you 5 turns)
Favor 75:  Corruption Touch — 15% chance on hit to make non-boss enemy fight for you
Favor 100: Walk Between Worlds — active; teleport to any visible tile; no cooldown
```

**Azathoth**
```
Favor 25:  Void Leak — mana regenerates 1 MP/turn passively
Favor 50:  Null Armor — 10% of incoming magic damage is absorbed as mana
Favor 75:  Entropy Field — nearby enemies take 2 void damage/turn (no action required)
Favor 100: Reality Shatter — active; destroys all terrain in 5-tile radius, damages all enemies in range
```

**Yog-Sothoth**
```
Favor 25:  Gate Sense — auto-identify all items on pickup
Favor 50:  Through the Gates — reveal entire current floor map instantly
Favor 75:  The Eternal Word — active (1/floor); instantly kill one non-boss enemy
Favor 100: All-In-One — act twice per turn for 10 turns (once per dungeon)
```

**Hastur**
```
Favor 25:  King's Gaze — +15% ranged accuracy in dim/dark tiles
Favor 50:  Yellow Aura — enemies within FOV have 10% chance to cower each turn
Favor 75:  Sign of Hastur — active; all enemies in room flee for 8 turns
Favor 100: The King Descends — Hastur's Avatar is summoned as your ally for current floor
```

**Dagon**
```
Favor 25:  Gills — no drowning; water tiles cost no movement penalty
Favor 50:  Tide Caller — water tiles spread 1 tile per 10 turns on current floor
Favor 75:  Crushing Depth — active; flood current room, push all enemies back 3 tiles, 20 water damage
Favor 100: Abyssal Form — active (1/run); become a Deep One hybrid for 30 turns (+100 HP, +20 dmg, water abilities)
```

### Divine Wrath Events (Anger-based)

When a god's Anger hits a threshold, they act against you directly.

| Anger | Event |
|-------|-------|
| 40    | Warning: ominous message; minor curse (e.g., -5 to hit for 20 turns) |
| 60    | Monster summon wave (see per-god table above) |
| 75    | Stat drain curse: -10 to a random stat until next floor |
| 90    | Elite monster summon wave (see per-god table above) |
| 100   | God's champion spawns — a named, buffed version of their favored monster type; tracks you across floors |

## Spell Database

### Standard Spells

| Spell | MP | Target | Effect | Notes |
|-------|----|--------|--------|-------|
| **Magic Bolt** | 5 | Single | 10 magic damage | Basic ranged attack |
| **Fireball** | 12 | Area (3r) | 25 fire damage, ignites tiles | Starts fires |
| **Frost Nova** | 10 | Area (2r) | 15 cold damage, freeze 3 turns | Extinguishes fire |
| **Lightning Strike** | 10 | Chain | 20 lightning, chains to 2 more | Conducts through water |
| **Mage Armor** | 8 | Self | +15 AC for 20 turns | — |
| **Void Bolt** | 8 | Single | 18 void damage | Ignores armor |
| **Healing Word** | 10 | Self | Restore 20 HP | — |
| **Blink** | 6 | Self | Teleport to random adjacent tile | Good for escapes |
| **Phase Step** | 14 | Self | Pass through walls for 5 turns | Cannot end turn inside wall |
| **Mirror Image** | 12 | Self | Create 3 decoys; enemies target randomly | Decoys have 1 HP |
| **Raise Dead** | 15 | Corpse | Animate a killed monster as ally for 15 turns | Any monster tier |
| **Bone Spear** | 9 | Line | 22 physical+bone damage, pierces all in line | — |
| **Petrify** | 16 | Single | Turns enemy to stone for 5 turns (stunned, +50% dmg) | Non-boss only |
| **Gravity Well** | 14 | Area (4r) | Pull all enemies 3 tiles toward caster | Can pin to walls |
| **Blood Boil** | 18 | Single | 35 internal damage over 4 turns; bypasses armor | Nasty |
| **Wither** | 12 | Single | Reduce enemy max HP by 20% permanently | Stacks |
| **Eldritch Drain** | 10 | Single | Steal 15 HP and 5 sanity from enemy | Life-steal |
| **Swarm of Rats** | 10 | Area | Summon 2d4 rats that harry enemies for 8 turns | Cheap summon |
| **Static Field** | 14 | Self (aura) | Zap all adjacent enemies for 8 lightning/turn for 5 turns | Melee deterrent |
| **Silence** | 8 | Area (3r) | Prevents all spellcasting in radius for 6 turns | Affects player too |
| **Shadow Step** | 10 | Self | Teleport behind target enemy | Backstab setup |
| **Entangle** | 9 | Area (2r) | Root enemies in place for 4 turns | Roots in vines |
| **Summon Horror** | 25 | Room | Summon 1 random Lovecraftian creature (tier 4-6) | Uncontrolled — may attack you |
| **Death Word** | 20 | Single | Instantly kills enemy below 20% HP | Does nothing above threshold |
| **Polymorph** | 22 | Single | Transform enemy into harmless creature for 10 turns | Drops current weapon |
| **Time Echo** | 30 | Self | Undo last move (rewind one turn) | 1/floor |

### Super Spells

Super spells are high-cost, dramatic, level-wide effects. They appear with `[SUPER]` in the Spellbook and cost significant mana.

| Spell | MP | Effect |
|-------|----|--------|
| **Eye in the Sky** | 50 | Reveals every monster (name, HP, position) and item on the floor. Opens message log with full itemized report. |
| **Armageddon Rain** | 60 | Calls meteors on every room. 40-80 fire damage per enemy on floor; sets all non-water tiles on fire. Dangerous to self. |
| **Mass Petrification** | 55 | Every non-boss enemy on the floor is frozen for 8 turns. Massive tactical window. |
| **Reality Fracture** | 65 | Shatters all walls in a 6-tile radius. Spawns 1d4 random monsters from random tiers (chaos). |
| **The Dreaming Word** | 70 | All enemies on floor are subjected to a sanity check — each loses 1d8×5 sanity. May drive weaker enemies mad (fight each other). |
| **Summon Horde** | 50 | Summon 3d6 undead allies (random tier 1-3). They last until killed or floor change. |
| **Void Collapse** | 80 | Open a singularity at target tile. All entities within 5 tiles are pulled in and take 10 void damage/turn for 5 turns. Then it explodes (80 AoE). |

### Spell Acquisition

- Starting spells depend on class (see Character Creation)
- Scrolls found in dungeon teach new spells permanently
- Tomes (rare) teach super spells
- Some spells are locked behind sanity thresholds (cannot learn Summon Horror if Sanity > 50 — you aren't mad enough yet)
- Cultist and Mage learn spells faster; others can still learn, just find fewer spell scrolls

---

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

### Gold Drops

Every monster can drop gold. Intelligent or civilized monsters (goblins, cultists, orcs, elves, etc.) always carry some. Beasts and undead drop it rarely. Lovecraftian entities drop eldritch coins — strange currency that counts as gold but also has sanity cost to pick up (1-3 sanity).

| Monster Type | Gold Drop Chance | Gold Amount |
|--------------|-----------------|-------------|
| Beast (rats, bats, spiders) | 5% | 1-3 gp |
| Humanoid (goblin, orc, elf) | 80% | 5-25 gp |
| Undead (skeleton, zombie, ghoul) | 15% | 2-10 gp (old coins) |
| Cultist (any tier) | 90% | 10-50 gp |
| Deep One | 40% | 5-20 gp (strange coins, -1 sanity) |
| Lovecraftian (tier 5+) | 25% | 15-60 gp (eldritch coins, -1d4 sanity) |
| Avatar / Boss | 100% | 200-500 gp + artifact |

Gold is used at shops (found on floors 5, 10, 15, 20) and at mysterious merchants that occasionally appear. See Economy section.

### Tier 1-2 (Dungeon Levels 1-3)

| Monster | HP | Damage | Sanity | XP | Gold | Abilities |
|---------|-----|--------|--------|-----|------|-----------|
| Rat | 5 | 2 | 0 | 10 | — | Swarm (5+) |
| Giant Bat | 8 | 3 | 0 | 15 | — | Flying |
| Goblin | 15 | 5 | 0 | 25 | 5-15 | Numbers |
| Kobold | 12 | 4 | 0 | 20 | 3-10 | Traps |
| Skeleton | 20 | 8 | 2 | 35 | 2-6 | Undead |
| Zombie | 25 | 6 | 1 | 30 | — | Slow, rot |
| Giant Spider | 18 | 5 | 3 | 40 | — | Poison |
| Cave Spider | 12 | 3 | 2 | 30 | — | Web |

### Tier 3-4 (Dungeon Levels 4-7)

| Monster | HP | Damage | Sanity | XP | Gold | Abilities |
|---------|-----|--------|--------|-----|------|-----------|
| Orc | 35 | 12 | 0 | 75 | 10-30 | Berserk |
| Dark Elf | 30 | 14 | 2 | 80 | 15-40 | Magic (-10 HP) |
| Ghoul | 40 | 10 | 5 | 90 | 5-15 | Paralysis |
| Ogre | 60 | 18 | 3 | 100 | 20-50 | Throw rocks |
| Dark Cultist | 35 | 10 | 8 | 100 | 15-45 | Summon |
| Worm | 25 | 15 | 4 | 85 | — | Burrow |
| Fire Vampire | 25 | 15 | 5 | 95 | 10-25 | Life drain |
| Shadow | 20 | 8 | 10 | 110 | — | Invisibility |

### Tier 5-6 (Dungeon Levels 8-12)

| Monster | HP | Damage | Sanity | XP | Gold | Abilities |
|---------|-----|--------|--------|-----|------|-----------|
| Deep One | 50 | 18 | 15 | 200 | 10-30* | Amphibious, Devolve |
| Mi-Go | 55 | 20 | 20 | 220 | 20-40* | Flying, Brain harvest |
| Ghast | 45 | 22 | 12 | 200 | 5-15 | Stench, Rot |
| Wight | 60 | 15 | 15 | 210 | 10-20 | Level drain |
| Phantom | 35 | 12 | 25 | 230 | — | Possession |
| Byakhee | 50 | 18 | 10 | 200 | 15-30* | Flying, Poison |
| Hunting Horror | 40 | 25 | 30 | 250 | — | Constrict |
| Star-Spawn (Lesser) | 70 | 25 | 20 | 250 | 30-60* | Mythos magic |

*Eldritch coins — picking up costs 1d4 sanity.

### Tier 7-8 (Dungeon Levels 13-18)

| Monster | HP | Damage | Sanity | XP | Gold | Abilities |
|---------|-----|--------|--------|-----|------|-----------|
| Elder Thing | 80 | 30 | 25 | 400 | 40-80* | Sonic attack |
| Shoggoth | 100 | 35 | 35 | 450 | — | Regeneration, Acid |
| High Cultist | 70 | 25 | 30 | 380 | 50-120 | Summon, Sanity blast |
| Chthonian | 90 | 28 | 20 | 400 | — | Burrow, Ambush |
| Fire Elemental | 70 | 35 | 5 | 350 | — | Fire immunity |
| Vampire Lord | 120 | 30 | 25 | 450 | 80-150 | Domination |
| Flying Polyp | 80 | 25 | 40 | 480 | 30-60* | Telekinesis |
| Serpent of N'kai | 95 | 32 | 35 | 500 | 50-100* | Hypnosis |

### Tier 9-10 (Dungeon Levels 19-25)

| Monster | HP | Damage | Sanity | XP | Gold | Abilities |
|---------|-----|--------|--------|-----|------|-----------|
| High Priest | 150 | 45 | 50 | 1000 | 200-400 | Summon, Miracles |
| Star-Spawn (Greater) | 180 | 55 | 60 | 1200 | 100-200* | Reality warp |
| Dagon's Avatar | 200 | 50 | 55 | 1300 | 300* | Water control |
| Hastur's Avatar | 220 | 60 | 70 | 1400 | 300* | Yellow Sign |
| Nyarlathotep's Avatar | 250 | 65 | 75 | 1500 | 400* | Chaos magic |
| Shoggoth (Elder) | 300 | 70 | 80 | 1800 | — | Hive mind |
| Great Old One | 500 | 100 | 100 | 5000 | 1000* | Reality destruction |
| Cthulhu | 666 | 150 | 200 | 10000 | 0 (what would you buy?) | Dream revival |

### Economy

Gold is found on monsters, in chests, and scattered on floors. It is spent at:

| Vendor | Location | Sells |
|--------|----------|-------|
| **Shop** | Fixed floors (5, 10, 15, 20) | Weapons, armor, potions, scrolls |
| **Black Market** | Random (20% chance per floor) | Illegal sanity drugs, cursed items, forbidden tomes |
| **Wandering Merchant** | Random encounter (10% per floor) | Mixed stock, high prices |
| **Cultist Fence** | Floors 8+ | Eldritch artifacts, spell scrolls, Deep One gear |

Eldritch coins (from Lovecraftian monsters) are worth 2× standard gold at the Cultist Fence but only 1× elsewhere. Shops on deeper floors stock higher-tier goods. Prices scale with dungeon depth.

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

Maps are larger than the screen. The screen is always 80x25; the map is 120x60 (or larger on deeper floors). A camera viewport tracks the player and renders only the visible portion.

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

### Camera / Viewport System

The game area occupies 80×18 tiles on screen (leaving 7 rows for the HUD and message log). The map can be up to **120×60 tiles**, with size scaling by dungeon depth.

```
Map dimensions by floor:
  Floors  1-5:   100×50
  Floors  6-15:  120×60
  Floors 16-25:  140×70

Viewport: 80×18 tiles, centered on the player.
Camera clamps at map edges (no black borders — just the wall tiles).
```

**Camera follows player every move.** Only changed cells are redrawn (double-buffer). The minimap (10×5 in the HUD area) always shows the full floor layout with player position marked.

### Sanity-Triggered Map Mutations

The map is static under normal conditions. At **Broken** sanity (0-9), reality starts to come apart:

| Trigger | Mutation |
|---------|----------|
| Sanity drops into Broken | Walls shift: 1d6 random wall tiles become floors, 1d6 floor tiles become walls |
| Each 20 turns at Broken | Rooms re-connect randomly: a new passage may open or close somewhere on the floor |
| Sanity stays Broken for 50+ turns | A previously-visited room has its contents reshuffled (items, monsters repositioned) |
| Hallucination peak (sanity 0-2) | The stairs may appear to move (fake stair tiles appear at random positions) |
| Returning to Stable from Broken | All mutations from this floor **persist** — the map was changed, you just didn't notice |

Mutations are local and cosmetic enough that the floor remains completable, but navigation becomes unreliable. The minimap reflects real layout; hallucinated tiles only affect the main view.

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

✅ Pure ASCII console interface with RogueSharp; scrolling camera over large maps (up to 140×70)
✅ Sanity-triggered map mutations (walls shift, passages open/close at Broken sanity)
✅ Monster gold drops + economy (shops, black market, cultist fence, eldritch coins)
✅ Ironman permadeath
✅ Dice-rolled stats with racial modifiers  
✅ Class selection menu with starting equipment
✅ Dual HP/Mana + Sanity resource system
✅ Sanity healing items with addiction drawbacks
✅ 8 races (standard + Lovecraftian)
✅ 6 classes (including Cultist, Gunslinger, Investigator)
✅ 50+ weapons (medieval through dieselpunk/lovecraftian)
✅ 40+ monsters (Lovecraftian horror hierarchy)
✅ 6 gods with reactive favor/anger mechanics, per-god preferences (semi-random per run), divine interventions, and monster summons
✅ 25+ spells including super spells with dramatic level-wide effects
✅ JSON save system
✅ Component-based architecture for extensibility