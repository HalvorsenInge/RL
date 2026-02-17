using EldritchDungeon.Entities;

namespace EldritchDungeon.Systems;

public class ReligionSystem
{
    private readonly Action<string> _log;

    public ReligionSystem(Action<string> log)
    {
        _log = log;
    }

    public void Pray(Player player)
    {
        if (player.Religion.CurrentGod == null)
        {
            _log("You have no god to pray to.");
            return;
        }

        player.Religion.AddFavor(2);
        _log($"You pray to {player.Religion.CurrentGod}. Favor increased to {player.Religion.Favor}.");
    }

    public void OnKill(Player player, Monster monster)
    {
        if (player.Religion.CurrentGod == null)
            return;

        player.Religion.AddFavor(3);
        _log($"Your kill pleases {player.Religion.CurrentGod}. Favor: {player.Religion.Favor}.");
    }
}
