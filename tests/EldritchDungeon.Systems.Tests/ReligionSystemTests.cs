using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;

namespace EldritchDungeon.Systems.Tests;

public class ReligionSystemTests
{
    private readonly List<string> _messages = new();
    private readonly ReligionSystem _religion;

    public ReligionSystemTests()
    {
        _religion = new ReligionSystem(msg => _messages.Add(msg));
    }

    private static Player CreateTestPlayer(GodType? god = null)
    {
        var player = new Player { Name = "Tester" };
        player.InitializeComponents();
        if (god.HasValue)
            player.Religion.SetGod(god.Value);
        return player;
    }


    [Fact]
    public void OnKill_WithGod_IncreasesFavorBy3()
    {
        var player = CreateTestPlayer(GodType.Dagon);
        var monster = new Monster { Name = "Rat", Tier = 1 };

        _religion.OnKill(player, monster);

        Assert.Equal(3, player.Religion.Favor);
    }

    [Fact]
    public void OnKill_WithoutGod_NoEffect()
    {
        var player = CreateTestPlayer();
        var monster = new Monster { Name = "Rat", Tier = 1 };

        _religion.OnKill(player, monster);

        Assert.Equal(0, player.Religion.Favor);
    }

}
