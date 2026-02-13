namespace EldritchDungeon.Entities.Components;

public class HealthComponent : IComponent
{
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public bool IsDead => CurrentHp <= 0;

    public void Initialize() { }

    public void SetMaxHp(int baseHp, int conModifier, double racialMultiplier)
    {
        MaxHp = (int)((baseHp + conModifier * 5) * racialMultiplier);
        if (MaxHp < 1) MaxHp = 1;
        CurrentHp = MaxHp;
    }

    public void TakeDamage(int amount)
    {
        CurrentHp = Math.Max(0, CurrentHp - amount);
    }

    public void Heal(int amount)
    {
        CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
    }
}
