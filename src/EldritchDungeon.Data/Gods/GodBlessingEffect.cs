namespace EldritchDungeon.Data.Gods;

public enum BlessingType
{
    HpRegenPerTurn,
    ManaRegenPerTurn,
    DamageBonus,
    CritChanceBonus,    // fraction, e.g. 0.05 per tier
    SanityResistBonus,  // fraction, e.g. 0.10 per tier
    XpBonusPercent      // fraction, e.g. 0.10 per tier
}

public class GodBlessingEffect
{
    public BlessingType Type { get; init; }

    /// <summary>
    /// Multiplied by the player's current PowerTier (1–4) to get the actual value.
    /// Flat for HP/Mana/Damage; fraction for Crit/SanityResist/XP.
    /// </summary>
    public double BaseValuePerTier { get; init; }
}
