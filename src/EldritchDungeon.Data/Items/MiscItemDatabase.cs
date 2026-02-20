using EldritchDungeon.Core;
using EldritchDungeon.Entities.Items;

namespace EldritchDungeon.Data.Items;

public static class MiscItemDatabase
{
    private static readonly Dictionary<string, Item> _items = new()
    {
        ["15 Arrows"] = new Item { Name = "15 Arrows", Type = ItemType.Ammo, Glyph = '/', Value = 5 },
        ["24 Bullets"] = new Item { Name = "24 Bullets", Type = ItemType.Ammo, Glyph = '*', Value = 12 },
        ["18 Bullets"] = new Item { Name = "18 Bullets", Type = ItemType.Ammo, Glyph = '*', Value = 9 },
        ["Lockpick Set"] = new Item { Name = "Lockpick Set", Type = ItemType.Tool, Glyph = 'k', Value = 20 },
        ["Spellbook (Fireball)"] = new Item { Name = "Spellbook (Fireball)", Type = ItemType.Scroll, Glyph = '+', Value = 50 },
        ["Chalk (Summoning)"] = new Item { Name = "Chalk (Summoning)", Type = ItemType.Tool, Glyph = '~', Value = 10 },
        ["Notebook"] = new Item { Name = "Notebook", Type = ItemType.Tool, Glyph = 'n', Value = 5 },
        ["Magnifying Glass"] = new Item { Name = "Magnifying Glass", Type = ItemType.Tool, Glyph = 'o', Value = 15 },
    };

    public static Item Get(string name) => _items[name];

    public static bool TryGet(string name, out Item item) => _items.TryGetValue(name, out item!);
}
