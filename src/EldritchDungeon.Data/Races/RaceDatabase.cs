using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Races;

public static class RaceDatabase
{
    private static readonly Dictionary<RaceType, RaceDefinition> _races = new()
    {
        [RaceType.Human] = new RaceDefinition
        {
            Type = RaceType.Human, Name = "Human",
            StrMod = 0, DexMod = 0, ConMod = 0, IntMod = 0, WisMod = 0, ChaMod = 0,
            HpMultiplier = 1.0, ManaMultiplier = 1.0, SanityMultiplier = 1.0
        },
        [RaceType.Elf] = new RaceDefinition
        {
            Type = RaceType.Elf, Name = "Elf",
            StrMod = -1, DexMod = 2, ConMod = -1, IntMod = 1, WisMod = 0, ChaMod = 0,
            HpMultiplier = 0.9, ManaMultiplier = 1.2, SanityMultiplier = 1.0
        },
        [RaceType.Dwarf] = new RaceDefinition
        {
            Type = RaceType.Dwarf, Name = "Dwarf",
            StrMod = 2, DexMod = -1, ConMod = 2, IntMod = 0, WisMod = 0, ChaMod = -1,
            HpMultiplier = 1.3, ManaMultiplier = 0.8, SanityMultiplier = 1.1
        },
        [RaceType.Halfling] = new RaceDefinition
        {
            Type = RaceType.Halfling, Name = "Halfling",
            StrMod = -2, DexMod = 2, ConMod = 0, IntMod = 0, WisMod = 0, ChaMod = 1,
            HpMultiplier = 0.8, ManaMultiplier = 1.1, SanityMultiplier = 1.3
        },
        [RaceType.Orc] = new RaceDefinition
        {
            Type = RaceType.Orc, Name = "Orc",
            StrMod = 3, DexMod = -1, ConMod = 2, IntMod = -2, WisMod = -1, ChaMod = -1,
            HpMultiplier = 1.4, ManaMultiplier = 0.6, SanityMultiplier = 0.7
        },
        [RaceType.DeepOneHybrid] = new RaceDefinition
        {
            Type = RaceType.DeepOneHybrid, Name = "Deep One Hybrid",
            StrMod = 1, DexMod = 1, ConMod = 1, IntMod = 0, WisMod = 0, ChaMod = -1,
            HpMultiplier = 1.1, ManaMultiplier = 1.0, SanityMultiplier = 0.5
        },
        [RaceType.HalfMad] = new RaceDefinition
        {
            Type = RaceType.HalfMad, Name = "Half-Mad",
            StrMod = 0, DexMod = 0, ConMod = -1, IntMod = 2, WisMod = -1, ChaMod = 0,
            HpMultiplier = 0.9, ManaMultiplier = 1.3, SanityMultiplier = 0.3
        },
        [RaceType.SerpentFolk] = new RaceDefinition
        {
            Type = RaceType.SerpentFolk, Name = "Serpent Folk",
            StrMod = 0, DexMod = 1, ConMod = 0, IntMod = 1, WisMod = 0, ChaMod = 0,
            HpMultiplier = 1.0, ManaMultiplier = 0.9, SanityMultiplier = 0.8
        }
    };

    public static RaceDefinition Get(RaceType type) => _races[type];

    public static IReadOnlyDictionary<RaceType, RaceDefinition> GetAll() => _races;
}
