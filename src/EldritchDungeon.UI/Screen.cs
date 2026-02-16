namespace EldritchDungeon.UI;

public abstract class Screen
{
    public abstract void Render();
    public abstract void HandleInput(ConsoleKeyInfo keyInfo);
}
