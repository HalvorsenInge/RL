using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Components;

public class SanityComponent : IComponent
{
    public int CurrentSanity { get; set; }
    public int MaxSanity { get; set; }
    public double InsanityResist { get; set; }

    public SanityState State
    {
        get
        {
            if (CurrentSanity >= GameConstants.SanityStableMin) return SanityState.Stable;
            if (CurrentSanity >= GameConstants.SanityFracturedMin) return SanityState.Fractured;
            if (CurrentSanity >= GameConstants.SanityUnravelingMin) return SanityState.Unraveling;
            return SanityState.Broken;
        }
    }

    public void Initialize() { }

    public void SetMaxSanity(int wisModifier, double racialMultiplier)
    {
        MaxSanity = (int)((GameConstants.MaxSanity + wisModifier * 10) * racialMultiplier);
        if (MaxSanity < 1) MaxSanity = 1;
        CurrentSanity = MaxSanity;
    }

    public void LoseSanity(int amount)
    {
        int effective = (int)(amount * (1.0 - InsanityResist));
        CurrentSanity = Math.Max(0, CurrentSanity - effective);
    }

    public void RestoreSanity(int amount)
    {
        CurrentSanity = Math.Min(MaxSanity, CurrentSanity + amount);
    }
}
