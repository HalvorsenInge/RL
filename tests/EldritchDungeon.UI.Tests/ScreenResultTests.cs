using EldritchDungeon.UI;

namespace EldritchDungeon.UI.Tests;

public class ScreenResultTests
{
    [Fact]
    public void HelpScreen_EscapeReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new HelpScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void HelpScreen_OtherKeyReturnsNone()
    {
        var renderer = new ASCIIRenderer();
        var screen = new HelpScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

        Assert.Equal(ScreenResult.None, result);
    }

    [Fact]
    public void CharacterScreen_EscapeReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new CharacterScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void CharacterScreen_IOpensInventory()
    {
        var renderer = new ASCIIRenderer();
        var screen = new CharacterScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('i', ConsoleKey.I, false, false, false));

        Assert.Equal(ScreenResult.OpenInventory, result);
    }

    [Fact]
    public void CharacterScreen_ROpensReligion()
    {
        var renderer = new ASCIIRenderer();
        var screen = new CharacterScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false));

        Assert.Equal(ScreenResult.OpenReligion, result);
    }

    [Fact]
    public void CharacterScreen_QuestionMarkOpensHelp()
    {
        var renderer = new ASCIIRenderer();
        var screen = new CharacterScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('?', ConsoleKey.Oem2, true, false, false));

        Assert.Equal(ScreenResult.OpenHelp, result);
    }

    [Fact]
    public void MessageScreen_EscapeReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new MessageScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void LookScreen_EscapeReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new LookScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void GameScreen_HandleInputReturnsNone()
    {
        var renderer = new ASCIIRenderer();
        var screen = new GameScreen(renderer);

        var result = screen.HandleInput(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

        Assert.Equal(ScreenResult.None, result);
    }
}
