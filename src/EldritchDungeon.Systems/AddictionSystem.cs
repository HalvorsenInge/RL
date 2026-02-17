using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Systems;

public class AddictionSystem
{
    private readonly Action<string> _log;
    private int _turnsSinceLastWarning;

    public AddictionSystem(Action<string> log)
    {
        _log = log;
    }

    public void OnConsume(Player player, Consumable consumable)
    {
        if (consumable.AddictionRisk <= 0)
            return;

        player.AddictionLevel = Math.Min(
            GameConstants.MaxAddiction,
            player.AddictionLevel + consumable.AddictionRisk);

        if (player.AddictionLevel >= GameConstants.AddictionThreshold)
            _log("You feel a growing dependency...");
    }

    public void Update(Player player)
    {
        if (player.AddictionLevel < GameConstants.AddictionThreshold)
            return;

        if (!player.StatusEffects.HasEffect(StatusEffectType.Withdrawal))
        {
            player.StatusEffects.AddEffect(StatusEffectType.Withdrawal, 1, 1);
        }

        _turnsSinceLastWarning++;
        if (_turnsSinceLastWarning >= 10)
        {
            _turnsSinceLastWarning = 0;
            _log("You crave the substances that ease your mind...");
        }
    }
}
