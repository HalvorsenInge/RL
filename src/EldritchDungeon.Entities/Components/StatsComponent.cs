using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Components;

public class StatsComponent : IComponent
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Charisma { get; set; }

    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; } = 100;

    public void Initialize() { }

    public void SetBaseScores(int[] scores)
    {
        Strength = scores[0];
        Dexterity = scores[1];
        Constitution = scores[2];
        Intelligence = scores[3];
        Wisdom = scores[4];
        Charisma = scores[5];
    }

    public void ApplyRacialModifiers(int str, int dex, int con, int intel, int wis, int cha)
    {
        Strength += str;
        Dexterity += dex;
        Constitution += con;
        Intelligence += intel;
        Wisdom += wis;
        Charisma += cha;
    }

    public int GetModifier(StatType stat)
    {
        return Dice.GetModifier(GetStat(stat));
    }

    public int GetStat(StatType stat) => stat switch
    {
        StatType.Strength => Strength,
        StatType.Dexterity => Dexterity,
        StatType.Constitution => Constitution,
        StatType.Intelligence => Intelligence,
        StatType.Wisdom => Wisdom,
        StatType.Charisma => Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(stat))
    };
}
