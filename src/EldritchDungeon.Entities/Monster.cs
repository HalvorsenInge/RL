using EldritchDungeon.Core;

namespace EldritchDungeon.Entities;

public class Monster : Actor
{
    public int Tier { get; set; }
    public int Damage { get; set; }
    public int XpValue { get; set; }
    public int SanityDamage { get; set; }

    // Gold drop (set from MonsterDefinition at spawn time)
    public int GoldMin { get; set; }
    public int GoldMax { get; set; }
    public double GoldDropChance { get; set; }
    public bool IsEldritchCoin { get; set; }

    // Summoning fields
    public bool IsSummoned { get; set; }
    public SummonedDisposition Disposition { get; set; } = SummonedDisposition.Hostile;
    public int SummonDurationLeft { get; set; } = -1; // -1 = permanent
}
