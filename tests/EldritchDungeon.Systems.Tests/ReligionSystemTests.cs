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
    public void Pray_WithGod_IncreasesFavorBy2()
    {
        var player = CreateTestPlayer(GodType.Cthulhu);

        _religion.Pray(player);

        Assert.Equal(2, player.Religion.Favor);
        Assert.Contains(_messages, m => m.Contains("Cthulhu"));
    }

    [Fact]
    public void Pray_WithoutGod_NoEffect()
    {
        var player = CreateTestPlayer();

        _religion.Pray(player);

        Assert.Equal(0, player.Religion.Favor);
        Assert.Contains(_messages, m => m.Contains("no god"));
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

    [Fact]
    public void Pray_MultipleTimes_AccumulatesFavor()
    {
        var player = CreateTestPlayer(GodType.Hastur);

        _religion.Pray(player);
        _religion.Pray(player);
        _religion.Pray(player);

        Assert.Equal(6, player.Religion.Favor);
    }
}
