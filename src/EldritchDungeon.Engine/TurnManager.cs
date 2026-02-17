using EldritchDungeon.Core;

namespace EldritchDungeon.Engine;

public class TurnManager
{
    private readonly GameEngine _engine;

    public TurnManager(GameEngine engine)
    {
        _engine = engine;
    }

    public void ProcessTurn()
    {
        var player = _engine.Player;
        var map = _engine.Map;

        if (player == null || map == null)
            return;

        // 1. Status effects on player
        _engine.StatusEffectSystem.ProcessActor(player);

        // 2. Check sanity from visible monsters
        _engine.SanitySystem.Update(map);

        // 3. AI moves/attacks monsters
        _engine.AISystem.Update(map);

        // 4. Addiction check
        _engine.AddictionSystem.Update(player);

        // 5. Update FOV
        map.UpdateFov(player.X, player.Y, GameConstants.DefaultFovRadius);

        // 6. Check player death
        if (player.Health.IsDead)
        {
            _engine.Log.Add("You have died! Game over.");
            _engine.OnPlayerDeath();
        }
    }
}
