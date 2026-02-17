using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.UI;

namespace EldritchDungeon.UI.Tests;

public class ReligionScreenTests
{
    private static Player CreateTestPlayer()
    {
        Dice.SetSeed(42);
        return Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            new[] { 0, 0, 0, 0, 0, 0 }, 1.0, 1.0, 1.0, 40, 10, 50);
    }

    [Fact]
    public void SelectGod_SetsPlayerGod()
    {
        var renderer = new ASCIIRenderer();
        var screen = new ReligionScreen(renderer);
        var player = CreateTestPlayer();
        screen.SetPlayer(player);

        Assert.Null(player.Religion.CurrentGod);

        // First god (index 0 = Cthulhu), press Enter to select
        screen.HandleInput(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        Assert.NotNull(player.Religion.CurrentGod);
    }

    [Fact]
    public void NavigateDownThenSelect_ChoosesDifferentGod()
    {
        var renderer = new ASCIIRenderer();
        var screen = new ReligionScreen(renderer);
        var player = CreateTestPlayer();
        screen.SetPlayer(player);

        // Move down once then select
        screen.HandleInput(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false));
        screen.HandleInput(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        // Should have selected the second god (Nyarlathotep)
        Assert.Equal(GodType.Nyarlathotep, player.Religion.CurrentGod);
    }

    [Fact]
    public void EscapeReturnsClose_WhenHasGod()
    {
        var renderer = new ASCIIRenderer();
        var screen = new ReligionScreen(renderer);
        var player = CreateTestPlayer();
        player.Religion.SetGod(GodType.Dagon);
        screen.SetPlayer(player);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void EscapeReturnsClose_WhenNoGod()
    {
        var renderer = new ASCIIRenderer();
        var screen = new ReligionScreen(renderer);
        var player = CreateTestPlayer();
        screen.SetPlayer(player);

        var result = screen.HandleInput(new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));

        Assert.Equal(ScreenResult.Close, result);
    }

    [Fact]
    public void TabTogglesPantheonView_WhenHasGod()
    {
        var renderer = new ASCIIRenderer();
        var screen = new ReligionScreen(renderer);
        var player = CreateTestPlayer();
        player.Religion.SetGod(GodType.Cthulhu);
        screen.SetPlayer(player);

        // Tab should not close the screen
        var result = screen.HandleInput(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));

        Assert.Equal(ScreenResult.None, result);
    }
}
