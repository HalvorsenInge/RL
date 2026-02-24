using EldritchDungeon.Core;

namespace EldritchDungeon.Entities.Items;

public class ToolItem : Item
{
    public ToolEffect Effect { get; set; }
    public string EffectDescription { get; set; } = string.Empty;

    public ToolItem()
    {
        Type = ItemType.Tool;
        Glyph = '~';
    }
}
