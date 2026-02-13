using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Components;

public class StatusEffect
{
    public StatusEffectType Type { get; set; }
    public int RemainingTurns { get; set; }
    public int Magnitude { get; set; }
}

public class StatusEffectsComponent : IComponent
{
    public List<StatusEffect> ActiveEffects { get; set; } = new();

    public void Initialize() { }

    public void AddEffect(StatusEffectType type, int duration, int magnitude = 0)
    {
        ActiveEffects.Add(new StatusEffect
        {
            Type = type,
            RemainingTurns = duration,
            Magnitude = magnitude
        });
    }

    public bool HasEffect(StatusEffectType type)
    {
        return ActiveEffects.Any(e => e.Type == type);
    }

    public void Tick()
    {
        foreach (var effect in ActiveEffects)
            effect.RemainingTurns--;

        ActiveEffects.RemoveAll(e => e.RemainingTurns <= 0);
    }
}
