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
            AngerTrigger = "Betrayal, breaking promises",
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
            AngerTrigger = "Use holy magic",
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
            AngerTrigger = "Attack scholars",
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
            AngerTrigger = "Look at stars wrong",
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
            AngerTrigger = "Desecrate water",
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
