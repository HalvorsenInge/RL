namespace EldritchDungeon.Core;

public class GameConfig
{
    public int ScreenWidth { get; set; } = GameConstants.ScreenWidth;
    public int ScreenHeight { get; set; } = GameConstants.ScreenHeight;
    public bool IronmanMode { get; set; } = true;
    public string SaveDirectory { get; set; } = "saves";
}
