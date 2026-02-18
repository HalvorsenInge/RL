using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Gods;

public class GodWrathEffect
{
    /// <summary>Minimum anger value required for this tier to activate.</summary>
    public int AngerThreshold { get; init; }

    /// <summary>Probability per turn that the punishment fires (0.0–1.0).</summary>
    public double TriggerChance { get; init; }

    public int HpDamage { get; init; }
    public int SanityDamage { get; init; }

    /// <summary>Optional status effect applied on trigger. Null = none.</summary>
    public StatusEffectType? AppliedStatus { get; init; }
    public int StatusDuration { get; init; }

    public string Message { get; init; } = string.Empty;
}
