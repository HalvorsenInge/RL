using EldritchDungeon.Core;
using EldritchDungeon.Entities;

namespace EldritchDungeon.Systems;

public class LevelingSystem
{
    private readonly Action<string> _log;
    private static readonly StatType[] AllStats = Enum.GetValues<StatType>();

    public LevelingSystem(Action<string> log)
    {
        _log = log;
    }

    public void CheckLevelUp(Player player)
    {
        while (player.Stats.Experience >= player.Stats.ExperienceToNextLevel)
        {
            LevelUp(player);
        }
    }

    private void LevelUp(Player player)
    {
        player.Stats.Experience -= player.Stats.ExperienceToNextLevel;
        player.Stats.Level++;
        player.Stats.ExperienceToNextLevel = 100 * (int)Math.Pow(2, player.Stats.Level - 1);

        int hpGain = Math.Max(1, player.Stats.GetModifier(StatType.Constitution));
        player.Health.MaxHp += hpGain;
        player.Health.CurrentHp = player.Health.MaxHp;

        int manaGain = Math.Max(0, player.Stats.GetModifier(StatType.Intelligence));
        player.Mana.MaxMana += manaGain;
        player.Mana.CurrentMana = player.Mana.MaxMana;

        _log($"Level up! You are now level {player.Stats.Level}. (+{hpGain} HP, +{manaGain} Mana)");

        if (player.Stats.Level % 3 == 0)
        {
            var stat = AllStats[Dice.Roll(1, AllStats.Length) - 1];
            int current = player.Stats.GetStat(stat);
            player.Stats.SetStat(stat, current + 1);
            _log($"Your {stat} increases to {current + 1}!");
        }
    }
}
