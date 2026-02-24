using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Components;

public class ReligionComponent : IComponent
{
    public GodType? CurrentGod { get; set; }
    public int Favor { get; set; }
    public int Anger { get; set; }

    public int PowerTier
    {
        get
        {
            if (Favor >= GameConstants.FavorTier4) return 4;
            if (Favor >= GameConstants.FavorTier3) return 3;
            if (Favor >= GameConstants.FavorTier2) return 2;
            if (Favor >= GameConstants.FavorTier1) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Countdown (in turns) before a god summon wave can fire again.
    /// Prevents constant monster spam at high anger.
    /// </summary>
    public int SummonCooldown { get; set; }

    public void Initialize() { }

    public void SetGod(GodType god)
    {
        CurrentGod = god;
        Favor = 0;
        Anger = 0;
    }

    public void AddFavor(int amount)
    {
        Favor = Math.Clamp(Favor + amount, 0, GameConstants.MaxFavor);
    }

    public void DecreaseFavor( int amount )
    {
        if( Favor <= amount ) Anger = Math.Abs( Favor - amount );
        else Favor -= amount;
    }

    public void AddAnger(int amount)
    {
        Anger = Math.Clamp(Anger + amount, 0, GameConstants.MaxAnger);
    }

    public void DecreaseAnger( int amount )
    {
        if( Anger <= amount ) Anger = 0;
        else Anger -= amount;
    } 
}
