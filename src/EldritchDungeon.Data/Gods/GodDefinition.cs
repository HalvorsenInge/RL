using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Gods;

public class GodDefinition
{
    public GodType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string FavorBonus { get; init; } = string.Empty;
    public string AngerTrigger { get; init; } = string.Empty;
    public List<GodPower> Powers { get; init; } = new();

    /// <summary>Passive per-turn blessing granted by favor tier. Null if no per-turn effect.</summary>
    public GodBlessingEffect? Blessing { get; init; }

    /// <summary>
    /// Anger-threshold punishments, sorted ascending by AngerThreshold.
    /// ReligionSystem picks the highest tier whose threshold is met.
    /// </summary>
    public List<GodWrathEffect> WrathEffects { get; init; } = new();
}
