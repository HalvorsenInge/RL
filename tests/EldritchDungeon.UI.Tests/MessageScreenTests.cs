using EldritchDungeon.UI;

namespace EldritchDungeon.UI.Tests;

public class MessageScreenTests
{
    [Fact]
    public void ScrollDown_IncreasesOffset()
    {
        var renderer = new ASCIIRenderer();
        var screen = new MessageScreen(renderer);

        // Create enough messages to scroll
        var messages = new List<string>();
        for (int i = 0; i < 50; i++)
            messages.Add($"Message {i}");
        screen.SetMessages(messages);

        // Scroll up from bottom (messages start scrolled to bottom)
        screen.HandleInput(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));

        // Should not close
        var result = screen.HandleInput(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
        Assert.Equal(ScreenResult.None, result);
    }

    [Fact]
    public void HomeKey_ScrollsToTop()
    {
        var renderer = new ASCIIRenderer();
        var screen = new MessageScreen(renderer);

        var messages = new List<string>();
        for (int i = 0; i < 50; i++)
            messages.Add($"Message {i}");
        screen.SetMessages(messages);

        var result = screen.HandleInput(new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false));

        Assert.Equal(ScreenResult.None, result);
    }

    [Fact]
    public void Escape_ReturnsClose()
    {
        var renderer = new ASCIIRenderer();
        var screen = new MessageScreen(renderer);
        screen.SetMessages(new List<string>());

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void EmptyMessages_EscapeStillCloses()
    {
        var renderer = new ASCIIRenderer();
        var screen = new MessageScreen(renderer);
        screen.SetMessages(new List<string>());

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }
}
