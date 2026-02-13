using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Entities.Components;

public class EquipmentComponent : IComponent
{
    public Dictionary<EquipmentSlot, Item?> Slots { get; set; } = new();

    public void Initialize()
    {
        foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
            Slots[slot] = null;
    }

    public bool Equip(EquipmentSlot slot, Item item)
    {
        Slots[slot] = item;
        return true;
    }

    public Item? Unequip(EquipmentSlot slot)
    {
        if (!Slots.TryGetValue(slot, out var item)) return null;
        Slots[slot] = null;
        return item;
    }

    public Item? GetEquipped(EquipmentSlot slot)
    {
        return Slots.GetValueOrDefault(slot);
    }
}
