using EldritchDungeon.Core;
using EldritchDungeon.Entities;

namespace EldritchDungeon.Systems;

public class StatusEffectSystem
{
    private readonly Action<string> _log;

    public StatusEffectSystem(Action<string> log)
    {
        _log = log;
    }

    public void ProcessActor(Actor actor)
    {
        foreach (var effect in actor.StatusEffects.ActiveEffects.ToList())
        {
            switch (effect.Type)
            {
                case StatusEffectType.Poison:
                    int poisonDmg = effect.Magnitude;
                    actor.Health.TakeDamage(poisonDmg);
                    _log($"{actor.Name} takes {poisonDmg} poison damage!");
                    break;

                case StatusEffectType.Bleeding:
                    int bleedDmg = Math.Max(1, effect.Magnitude / 2);
                    actor.Health.TakeDamage(bleedDmg);
                    _log($"{actor.Name} takes {bleedDmg} bleeding damage!");
                    break;

                case StatusEffectType.Burning:
                    int burnDmg = effect.Magnitude;
                    actor.Health.TakeDamage(burnDmg);
                    _log($"{actor.Name} takes {burnDmg} burn damage!");
                    break;
            }
        }

        actor.StatusEffects.Tick();
    }

    public static bool IsIncapacitated(Actor actor)
    {
        return actor.StatusEffects.HasEffect(StatusEffectType.Stunned)
            || actor.StatusEffects.HasEffect(StatusEffectType.Paralyzed);
    }
}
