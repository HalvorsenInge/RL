namespace EldritchDungeon.Data.Monsters;

public class MonsterDefinition
{
    public string Name { get; init; } = string.Empty;
    public char Glyph { get; init; }
    public int Tier { get; init; }
    public int HP { get; init; }
    public int Damage { get; init; }
    public int SanityDamage { get; init; }
    public int XpValue { get; init; }
    public List<string> Abilities { get; init; } = new();
}
