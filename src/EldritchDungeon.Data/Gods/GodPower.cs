namespace EldritchDungeon.Data.Gods;

public class GodPower
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Tier { get; init; }
    public int FavorRequired { get; init; }
}
