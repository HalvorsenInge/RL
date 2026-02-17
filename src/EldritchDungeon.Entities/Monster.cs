namespace EldritchDungeon.Entities;

public class Monster : Actor
{
    public int Tier { get; set; }
    public int Damage { get; set; }
    public int XpValue { get; set; }
    public int SanityDamage { get; set; }
}
