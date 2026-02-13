using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Races;

public class RaceDefinition
{
    public RaceType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public int StrMod { get; init; }
    public int DexMod { get; init; }
    public int ConMod { get; init; }
    public int IntMod { get; init; }
    public int WisMod { get; init; }
    public int ChaMod { get; init; }
    public double HpMultiplier { get; init; }
    public double ManaMultiplier { get; init; }
    public double SanityMultiplier { get; init; }

    public int[] GetStatModifiers() => new[] { StrMod, DexMod, ConMod, IntMod, WisMod, ChaMod };
}
