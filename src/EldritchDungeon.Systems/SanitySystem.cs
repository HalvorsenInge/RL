using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.World;

namespace EldritchDungeon.Systems;

public class SanitySystem
{
    private readonly Action<string> _log;
    private readonly HashSet<string> _seenMonsterTypes = new();

    public SanitySystem(Action<string> log)
    {
        _log = log;
    }

    public void Update(DungeonMap map)
    {
        if (map.Player == null)
            return;

        var player = map.Player;

        foreach (var monster in map.Monsters)
        {
            if (monster.SanityDamage <= 0)
                continue;

            var tile = map.GetTile(monster.X, monster.Y);
            if (!tile.IsInFov)
                continue;

            if (_seenMonsterTypes.Contains(monster.Name))
                continue;

            _seenMonsterTypes.Add(monster.Name);
            player.Sanity.LoseSanity(monster.SanityDamage);
            _log($"The sight of the {monster.Name} shakes your sanity! (-{monster.SanityDamage})");
        }

        ApplySanityStateEffects(player);
    }

    private void ApplySanityStateEffects(Player player)
    {
        switch (player.Sanity.State)
        {
            case SanityState.Fractured:
                if (Dice.Roll(1, 10) == 1)
                    _log("Your vision blurs... reality seems unstable.");
                break;
            case SanityState.Unraveling:
                if (Dice.Roll(1, 5) == 1)
                    _log("You hear whispers from the walls...");
                break;
            case SanityState.Broken:
                if (Dice.Roll(1, 3) == 1)
                    _log("The madness consumes your thoughts!");
                break;
        }
    }

    public bool ShouldRandomAction(Player player)
    {
        return player.Sanity.State == SanityState.Broken && Dice.Roll(1, 10) == 1;
    }
}
