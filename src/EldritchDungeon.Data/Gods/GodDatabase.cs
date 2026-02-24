using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Gods;

public static class GodDatabase
{
    private static readonly Dictionary<GodType, GodDefinition> _gods = new()
    {
        [GodType.Cthulhu] = new GodDefinition
        {
            Type = GodType.Cthulhu, Name = "Cthulhu", Domain = "Dreams",
            FavorBonus = "HP Regen, Night Vision",
            AngerTrigger = "Attack Deep Ones",
            LovedMonsters = new List<string> { "Deep One", "Star-Spawn (Lesser)", "Star-Spawn (Greater)", "Dark Cultist" },
            HatedMonsters = new List<string> { "Skeleton", "Zombie", "Ghoul" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "Cthulhu's dream-voice screams — Deep Ones claw through the walls!",
                    Monsters = new List<(string, int)> { ("Deep One", 3), ("Dark Cultist", 1) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.06,
                    Message = "CTHULHU FHTAGN — a Star-Spawn erupts into reality!",
                    Monsters = new List<(string, int)> { ("Star-Spawn (Lesser)", 1), ("Deep One", 4) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.HpRegenPerTurn,
                BaseValuePerTier = 1.0   // 1/2/3/4 HP per turn at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, SanityDamage = 2,
                    Message = "Cthulhu stirs in his sleep. Your dreams turn dark. -2 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 4, SanityDamage = 4,
                    Message = "Cthulhu's dreaming eye falls upon you. -4 HP, -4 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 10, SanityDamage = 10,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 5,
                    Message = "Cthulhu's dreams devour your mind! -10 HP, -10 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Dreams", Tier = 1, FavorRequired = 25,
                    Description = "Regenerate HP while sleeping (2 HP/turn)"
                },
                new GodPower
                {
                    Name = "Tentacle Slam", Tier = 2, FavorRequired = 50,
                    Description = "30 void damage, stuns enemy"
                },
                new GodPower
                {
                    Name = "Cult Summon", Tier = 3, FavorRequired = 75,
                    Description = "Summon 2d4 cultists (friendly)"
                },
                new GodPower
                {
                    Name = "R'lyeh Rising", Tier = 4, FavorRequired = 100,
                    Description = "Earthquake, summons 10 undead"
                },
            }
        },

        [GodType.Nyarlathotep] = new GodDefinition
        {
            Type = GodType.Nyarlathotep, Name = "Nyarlathotep", Domain = "Chaos",
            FavorBonus = "Crit Chance, Loot",
            AngerTrigger = "Heals too much, stays orderly",
            LovedMonsters = new List<string> { "Shadow", "Fire Vampire", "Dark Elf" },
            HatedMonsters = new List<string> { "Goblin", "Kobold", "Rat" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "Nyarlathotep laughs. Shadows peel off the walls and attack!",
                    Monsters = new List<(string, int)> { ("Shadow", 3), ("Fire Vampire", 1) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.05,
                    Message = "The Crawling Chaos sends his avatar! RUN.",
                    Monsters = new List<(string, int)> { ("Nyarlathotep's Avatar", 1) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.CritChanceBonus,
                BaseValuePerTier = 0.05  // +5%/+10%/+15%/+20% crit at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, SanityDamage = 2,
                    Message = "Nyarlathotep's laughter echoes through your mind. -2 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 3, SanityDamage = 5,
                    AppliedStatus = StatusEffectType.Hallucinating, StatusDuration = 4,
                    Message = "Nyarlathotep twists reality around you. -3 HP, -5 Sanity, Hallucinating."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 8, SanityDamage = 12,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 6,
                    Message = "Nyarlathotep's mask slips. Chaos consumes you! -8 HP, -12 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Lucky Coin", Tier = 1, FavorRequired = 25,
                    Description = "Reroll one dice roll"
                },
                new GodPower
                {
                    Name = "Mimicry", Tier = 2, FavorRequired = 50,
                    Description = "Transform into monster for 20 turns"
                },
                new GodPower
                {
                    Name = "Corrupt", Tier = 3, FavorRequired = 75,
                    Description = "Convert enemy to follower (non-boss)"
                },
                new GodPower
                {
                    Name = "Walk Between", Tier = 4, FavorRequired = 100,
                    Description = "Teleport anywhere on map"
                },
            }
        },

        [GodType.Azathoth] = new GodDefinition
        {
            Type = GodType.Azathoth, Name = "Azathoth", Domain = "Void",
            FavorBonus = "Mana, Spell Power",
            AngerTrigger = "Use holy magic, avoids combat",
            LovedMonsters = new List<string> { "Flying Polyp", "Shoggeth", "Elder Thing" },
            HatedMonsters = new List<string> { "Goblin", "Rat", "Zombie" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "Azathoth's mindless hunger manifests — a polyp and shoggeths tear through!",
                    Monsters = new List<(string, int)> { ("Flying Polyp", 1), ("Shoggeth", 2) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.05,
                    Message = "VOID RUPTURE — an Elder Thing and Chthonian claw into reality!",
                    Monsters = new List<(string, int)> { ("Elder Thing", 1), ("Chthonian", 1) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.ManaRegenPerTurn,
                BaseValuePerTier = 1.0   // 1/2/3/4 MP per turn at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, SanityDamage = 3,
                    Message = "The void whispers of Azathoth's displeasure. -3 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 5, SanityDamage = 3,
                    Message = "Azathoth's mindless hunger gnaws at you. -5 HP, -3 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 12, SanityDamage = 8,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 5,
                    Message = "The Blind Idiot God notices you. Reality unravels! -12 HP, -8 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Mana Well", Tier = 1, FavorRequired = 25,
                    Description = "1 MP/turn regen"
                },
                new GodPower
                {
                    Name = "Void Blast", Tier = 2, FavorRequired = 50,
                    Description = "Pure void damage (ignores armor)"
                },
                new GodPower
                {
                    Name = "Nullify", Tier = 3, FavorRequired = 75,
                    Description = "Remove all magic from target"
                },
                new GodPower
                {
                    Name = "Reality Bend", Tier = 4, FavorRequired = 100,
                    Description = "Reroll entire room layout"
                },
            }
        },

        [GodType.YogSothoth] = new GodDefinition
        {
            Type = GodType.YogSothoth, Name = "Yog-Sothoth", Domain = "Knowledge",
            FavorBonus = "XP Gain, Identify",
            AngerTrigger = "Destroys books, rushes through floors",
            LovedMonsters = new List<string> { "Phantom", "Wight", "Serpent of N'kai" },
            HatedMonsters = new List<string> { "Goblin", "Kobold", "Giant Bat" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "Yog-Sothoth's gaze turns cold — Phantoms stream from the void!",
                    Monsters = new List<(string, int)> { ("Phantom", 2), ("Wight", 1) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.05,
                    Message = "The Gate opens — a Serpent of N'kai slithers through!",
                    Monsters = new List<(string, int)> { ("Serpent of N'kai", 1), ("Phantom", 2) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.XpBonusPercent,
                BaseValuePerTier = 0.10  // +10%/+20%/+30%/+40% XP at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, SanityDamage = 2,
                    Message = "Yog-Sothoth withholds his secrets. Ignorance gnaws at you. -2 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 3, SanityDamage = 6,
                    AppliedStatus = StatusEffectType.Hallucinating, StatusDuration = 3,
                    Message = "Yog-Sothoth floods your mind with forbidden knowledge. -3 HP, -6 Sanity, Hallucinating."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 6, SanityDamage = 15,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 5,
                    Message = "Yog-Sothoth tears open the gates of your mind! -6 HP, -15 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Ancient Knowledge", Tier = 1, FavorRequired = 25,
                    Description = "Auto-identify item"
                },
                new GodPower
                {
                    Name = "Clairvoyance", Tier = 2, FavorRequired = 50,
                    Description = "Reveal entire floor map"
                },
                new GodPower
                {
                    Name = "The Word", Tier = 3, FavorRequired = 75,
                    Description = "Kill any enemy (non-boss)"
                },
                new GodPower
                {
                    Name = "Omnipresence", Tier = 4, FavorRequired = 100,
                    Description = "Act twice per turn"
                },
            }
        },

        [GodType.Hastur] = new GodDefinition
        {
            Type = GodType.Hastur, Name = "Hastur", Domain = "Stars",
            FavorBonus = "Sanity Resist, Range",
            AngerTrigger = "Heals sanity, uses holy items",
            LovedMonsters = new List<string> { "Byakhee", "Hunting Horror", "High Cultist" },
            HatedMonsters = new List<string> { "Skeleton", "Zombie", "Goblin" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "The Yellow Sign burns in your mind — Byakhee descend from above!",
                    Monsters = new List<(string, int)> { ("Byakhee", 3), ("Hunting Horror", 1) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.05,
                    Message = "THE KING IN YELLOW DESCENDS. Hastur's Avatar walks the dungeon.",
                    Monsters = new List<(string, int)> { ("Hastur's Avatar", 1) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.SanityResistBonus,
                BaseValuePerTier = 0.10  // +10%/+20%/+30%/+40% sanity resist at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, SanityDamage = 3,
                    Message = "Hastur's yellow sign flickers at the edge of your vision. -3 Sanity."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 2, SanityDamage = 8,
                    AppliedStatus = StatusEffectType.Hallucinating, StatusDuration = 5,
                    Message = "The King in Yellow gazes down from alien stars. -2 HP, -8 Sanity, Hallucinating."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 5, SanityDamage = 15,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 6,
                    Message = "Hastur's yellow shroud descends upon you! -5 HP, -15 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Starlight", Tier = 1, FavorRequired = 25,
                    Description = "Light up room, +10% accuracy"
                },
                new GodPower
                {
                    Name = "Stars Emit", Tier = 2, FavorRequired = 50,
                    Description = "AoE star damage, -10 sanity"
                },
                new GodPower
                {
                    Name = "Yellow Sign", Tier = 3, FavorRequired = 75,
                    Description = "Enemies flee in terror"
                },
                new GodPower
                {
                    Name = "The King in Yellow", Tier = 4, FavorRequired = 100,
                    Description = "Summon Hastur's avatar"
                },
            }
        },

        [GodType.Dagon] = new GodDefinition
        {
            Type = GodType.Dagon, Name = "Dagon", Domain = "Deep",
            FavorBonus = "Water Abilities, Damage",
            AngerTrigger = "Use fire, kill Deep Ones",
            LovedMonsters = new List<string> { "Deep One", "Chthonian" },
            HatedMonsters = new List<string> { "Fire Elemental", "Fire Vampire", "Skeleton" },
            SummonWaves = new List<GodSummonWave>
            {
                new GodSummonWave
                {
                    AngerThreshold = 60, TriggerChance = 0.04,
                    Message = "Dagon's fury floods up from the deep — Deep Ones surge through the floor!",
                    Monsters = new List<(string, int)> { ("Deep One", 5) }
                },
                new GodSummonWave
                {
                    AngerThreshold = 90, TriggerChance = 0.05,
                    Message = "DAGON RISES. His Avatar emerges dripping from the dungeon walls.",
                    Monsters = new List<(string, int)> { ("Dagon's Avatar", 1), ("Deep One", 3) }
                }
            },
            Blessing = new GodBlessingEffect
            {
                Type = BlessingType.DamageBonus,
                BaseValuePerTier = 2.0   // +2/+4/+6/+8 flat damage at tiers 1–4
            },
            WrathEffects = new List<GodWrathEffect>
            {
                new GodWrathEffect
                {
                    AngerThreshold = 1, TriggerChance = 0.05, HpDamage = 2,
                    Message = "Dagon's scales ripple beneath your skin. -2 HP."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 34, TriggerChance = 0.15, HpDamage = 6, SanityDamage = 3,
                    AppliedStatus = StatusEffectType.Bleeding, StatusDuration = 4,
                    Message = "Dagon tears at your flesh from the deep. -6 HP, -3 Sanity, Bleeding."
                },
                new GodWrathEffect
                {
                    AngerThreshold = 67, TriggerChance = 0.30, HpDamage = 12, SanityDamage = 6,
                    AppliedStatus = StatusEffectType.Cursed, StatusDuration = 5,
                    Message = "Dagon's wrath rises from the abyss! -12 HP, -6 Sanity, Cursed."
                },
            },
            Powers = new List<GodPower>
            {
                new GodPower
                {
                    Name = "Water Breath", Tier = 1, FavorRequired = 25,
                    Description = "Breathe underwater"
                },
                new GodPower
                {
                    Name = "Flood", Tier = 2, FavorRequired = 50,
                    Description = "Fill room with shallow water"
                },
                new GodPower
                {
                    Name = "Tsunami", Tier = 3, FavorRequired = 75,
                    Description = "Massive water damage, push enemies"
                },
                new GodPower
                {
                    Name = "Abyssal Form", Tier = 4, FavorRequired = 100,
                    Description = "Transform into Deep One, +100 HP, +20 damage"
                },
            }
        },
    };

    public static GodDefinition Get(GodType type) => _gods[type];

    public static IReadOnlyDictionary<GodType, GodDefinition> GetAll() => _gods;
}
