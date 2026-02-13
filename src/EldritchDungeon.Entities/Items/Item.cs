using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Items;

public class Item
{
    public string Name { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public char Glyph { get; set; } = '?';
    public double Weight { get; set; }
    public int Value { get; set; }
}
