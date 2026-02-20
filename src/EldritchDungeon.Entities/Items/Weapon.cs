using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Items;

public class Weapon : Item
{
    public int Damage { get; set; }
    public int CritRangeMin { get; set; } = 20;
    public int CritMultiplier { get; set; } = 2;
    public int Speed { get; set; }
    public int Range { get; set; }
    public int MaxAmmo { get; set; }
    public int CurrentAmmo { get; set; }
    public WeaponCategory Category { get; set; }
    public string Special { get; set; } = string.Empty;
    public int MagicDamage { get; set; }
    public string EnchantmentName { get; set; } = string.Empty;

    public Weapon()
    {
        Type = ItemType.Weapon;
        Glyph = ')';
    }
}
