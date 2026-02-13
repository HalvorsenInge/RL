using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Classes;

public class ClassDefinition
{
    public ClassType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public int BaseHp { get; init; }
    public int BaseMana { get; init; }
    public int StartingGold { get; init; }
    public List<string> StartingWeapons { get; init; } = new();
    public string StartingArmor { get; init; } = string.Empty;
    public List<string> StartingItems { get; init; } = new();
}
