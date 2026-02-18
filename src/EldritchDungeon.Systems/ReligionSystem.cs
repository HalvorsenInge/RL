using EldritchDungeon.Core;
using EldritchDungeon.Data.Gods;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Components;

namespace EldritchDungeon.Systems;

public class ReligionSystem
{
    private readonly Action<string> _log;

    public ReligionSystem(Action<string> log)
    {
        _log = log;
    }

    public void Pray(Player player, int val)
    {
        if (player.Religion.CurrentGod == null)
        {
            _log("You have no god to pray to.");
            return;
        }

        Random rand = new Random();

        if(val > 50)
        {
            if(val > 95) 
            {
                player.Religion.AddFavor(4);
                player.Religion.DecreaseAnger(2);
                _log($"You pray to {player.Religion.CurrentGod}. {player.Religion.CurrentGod} loves the prayer. Favor increased to {player.Religion.Favor}.");
            }

            else
            {    
                player.Religion.AddFavor(2);
                _log($"You pray to {player.Religion.CurrentGod}. Favor increased to {player.Religion.Favor}.");
            }
        }

        else
        {
            if(val < 5) 
            {
                player.Religion.DecreaseFavor(10);
                player.Religion.AddAnger(5);
                _log($"You pray to {player.Religion.CurrentGod}. {player.Religion.CurrentGod} hates the prayer. Favor decreased to {player.Religion.Favor}.");
            }

            else
            {    
                player.Religion.DecreaseFavor(4);
                _log($"You pray to {player.Religion.CurrentGod}. Favor decreased to {player.Religion.Favor}.");
            }
        }
    }

    public void OnKill(Player player, Monster monster)
    {
        if (player.Religion.CurrentGod == null)
            return;

        player.Religion.AddFavor(3);
        _log($"Your kill pleases {player.Religion.CurrentGod}. Favor: {player.Religion.Favor}.");
    }

    /// <summary>
    /// Called each game turn. Reads blessing data from GodDatabase and applies
    /// per-turn effects (HP regen, mana regen) and syncs component values
    /// (sanity resist) that other systems read passively.
    /// </summary>
    public void ApplyBlessings(Player player)
    {
        if (player.Religion.CurrentGod == null || player.Religion.PowerTier == 0)
        {
            player.Sanity.InsanityResist = 0.0;
            return;
        }

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        var blessing = god.Blessing;

        if (blessing == null) return;

        double value = blessing.BaseValuePerTier * player.Religion.PowerTier;

        switch (blessing.Type)
        {
            case BlessingType.HpRegenPerTurn:
                if (!player.Health.IsDead)
                    player.Health.Heal((int)value);
                break;

            case BlessingType.ManaRegenPerTurn:
                player.Mana.Restore((int)value);
                break;

            case BlessingType.SanityResistBonus:
                // Sync so SanityComponent.LoseSanity picks it up automatically.
                player.Sanity.InsanityResist = value;
                break;

            // DamageBonus, CritChanceBonus, XpBonusPercent are passive — other
            // systems query GetBlessingValue() instead of being set here.
        }

        // Clear sanity resist if the god doesn't grant it.
        if (blessing.Type != BlessingType.SanityResistBonus)
            player.Sanity.InsanityResist = 0.0;
    }

    /// <summary>
    /// Called each game turn. Reads wrath effects from GodDatabase and fires
    /// the highest applicable anger-threshold punishment probabilistically.
    /// </summary>
    public void ApplyWrath(Player player)
    {
        if (player.Religion.CurrentGod == null || player.Religion.Anger == 0)
            return;

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        int anger = player.Religion.Anger;

        // Pick the highest-threshold effect whose threshold is met.
        var effect = god.WrathEffects
            .Where(e => anger >= e.AngerThreshold)
            .MaxBy(e => e.AngerThreshold);

        if (effect == null) return;

        if (new Random().NextDouble() < effect.TriggerChance)
        {
            if (effect.HpDamage > 0) player.Health.TakeDamage(effect.HpDamage);
            if (effect.SanityDamage > 0) player.Sanity.LoseSanity(effect.SanityDamage);
            if (effect.AppliedStatus.HasValue)
                player.StatusEffects.AddEffect(effect.AppliedStatus.Value, effect.StatusDuration);
            _log(effect.Message);
        }
    }

    /// <summary>
    /// Returns the current value of a passive blessing bonus for use by other
    /// systems (e.g. CombatSystem querying DamageBonus, LevelingSystem querying
    /// XpBonusPercent). Returns 0 if the player has no god, no blessing, or the
    /// blessing type doesn't match.
    /// </summary>
    public static double GetBlessingValue(Player player, BlessingType type)
    {
        if (player.Religion.CurrentGod == null || player.Religion.PowerTier == 0)
            return 0.0;

        var god = GodDatabase.Get(player.Religion.CurrentGod.Value);
        if (god.Blessing?.Type != type) return 0.0;

        return god.Blessing.BaseValuePerTier * player.Religion.PowerTier;
    }
}
