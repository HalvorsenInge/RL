using EldritchDungeon.Core;

namespace EldritchDungeon.Data.Summoning;

public class SummonableEntry
{
    public string Name { get; set; } = string.Empty;
    public char Glyph { get; set; }
    public int HP { get; set; }
    public int Damage { get; set; }
    public int Tier { get; set; }
    public int SanityDamage { get; set; }
    public int XpValue { get; set; }

    // Weighted chance for each disposition (higher = more likely)
    public int HostileWeight { get; set; } = 1;
    public int NeutralWeight { get; set; } = 1;
    public int FriendlyWeight { get; set; } = 1;

    // How many turns the summon lasts; -1 = until killed
    public int SummonDuration { get; set; } = -1;

    // Flavor text logged at summon and when disposition is revealed
    public string SummonMessage { get; set; } = string.Empty;
    public string HostileMessage { get; set; } = "It decides you are the enemy.";
    public string NeutralMessage { get; set; } = "It ignores you.";
    public string FriendlyMessage { get; set; } = "It seems willing to help.";
}
