using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Gods;

public class GodDefinition
{
    public GodType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string FavorBonus { get; init; } = string.Empty;
    public string AngerTrigger { get; init; } = string.Empty;
    public List<GodPower> Powers { get; init; } = new();
}
