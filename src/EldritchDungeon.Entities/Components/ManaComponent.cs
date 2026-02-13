namespace EldritchDungeon.Entities.Components;

public class ManaComponent : IComponent
{
    public int CurrentMana { get; set; }
    public int MaxMana { get; set; }

    public void Initialize() { }

    public void SetMaxMana(int baseMana, int intModifier, double racialMultiplier)
    {
        MaxMana = (int)((baseMana + intModifier * 5) * racialMultiplier);
        if (MaxMana < 0) MaxMana = 0;
        CurrentMana = MaxMana;
    }

    public bool Spend(int amount)
    {
        if (CurrentMana < amount) return false;
        CurrentMana -= amount;
        return true;
    }

    public void Restore(int amount)
    {
        CurrentMana = Math.Min(MaxMana, CurrentMana + amount);
    }
}
