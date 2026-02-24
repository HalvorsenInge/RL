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

    // ── Reactive behaviour (no worship required) ─────────────────────────────

    /// <summary>Monsters this god protects. Killing them angers the god.</summary>
    public List<string> LovedMonsters { get; init; } = new();

    /// <summary>Monsters this god despises. Killing them pleases the god.</summary>
    public List<string> HatedMonsters { get; init; } = new();

    /// <summary>
    /// Monster summon waves triggered at anger thresholds.
    /// Sorted ascending by AngerThreshold; highest applicable wave fires.
    /// </summary>
    public List<GodSummonWave> SummonWaves { get; init; } = new();

    /// <summary>Favor gained when player kills a HatedMonster.</summary>
    public int FavorOnHatedKill { get; init; } = 8;

    /// <summary>Anger gained when player kills a LovedMonster.</summary>
    public int AngerOnLovedKill { get; init; } = 10;
}
