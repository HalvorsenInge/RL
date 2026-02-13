using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Items;

public class Armor : Item
{
    public int ArmorClass { get; set; }
    public EquipmentSlot Slot { get; set; } = EquipmentSlot.Body;

    public Armor()
    {
        Type = ItemType.Armor;
        Glyph = '[';
    }
}
