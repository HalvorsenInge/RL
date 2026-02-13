using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Entities.Components;

public class InventoryComponent : IComponent
{
    public List<Item> Items { get; set; } = new();
    public int Capacity { get; set; } = 20;

    public void Initialize() { }

    public bool AddItem(Item item)
    {
        if (Items.Count >= Capacity) return false;
        Items.Add(item);
        return true;
    }

    public bool RemoveItem(Item item)
    {
        return Items.Remove(item);
    }
}
