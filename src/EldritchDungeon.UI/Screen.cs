namespace EldritchDungeon.UI;

public enum ScreenResult
{
    None,
    Close,
    OpenCharacter,
    OpenInventory,
    OpenReligion,
    OpenMessages,
    OpenHelp,
    OpenLook,
    OpenSpell,
    OpenShop,
    UseTool
}

public abstract class Screen
{
    public abstract void Render();
    public abstract ScreenResult HandleInput(ConsoleKeyInfo keyInfo);
}
