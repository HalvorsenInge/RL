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

    // Gold drops
    public int GoldMin { get; init; }
    public int GoldMax { get; init; }
    /// <summary>0.0–1.0 probability of dropping gold when slain.</summary>
    public double GoldDropChance { get; init; }
    /// <summary>If true, picking up the coins costs 1 sanity (eldritch coins).</summary>
    public bool IsEldritchCoin { get; init; }
}
