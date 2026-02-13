using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Items;

public class Consumable : Item
{
    public int HealAmount { get; set; }
    public int ManaAmount { get; set; }
    public int SanityAmount { get; set; }
    public int AddictionRisk { get; set; }

    public Consumable()
    {
        Type = ItemType.Consumable;
        Glyph = '!';
    }
}
