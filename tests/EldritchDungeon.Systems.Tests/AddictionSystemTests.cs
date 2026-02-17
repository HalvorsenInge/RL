using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Systems;

namespace EldritchDungeon.Systems.Tests;

public class AddictionSystemTests
{
    private readonly List<string> _messages = new();
    private readonly AddictionSystem _addiction;

    public AddictionSystemTests()
    {
        _addiction = new AddictionSystem(msg => _messages.Add(msg));
    }

    private static Player CreateTestPlayer()
    {
        var player = new Player { Name = "Tester" };
        player.InitializeComponents();
        return player;
    }

    [Fact]
    public void OnConsume_IncreasesAddictionLevel()
    {
        var player = CreateTestPlayer();
        var consumable = new Consumable
        {
            Name = "Mindcrust",
            SanityAmount = 50,
            AddictionRisk = 30
        };

        _addiction.OnConsume(player, consumable);

        Assert.Equal(30, player.AddictionLevel);
    }

    [Fact]
    public void OnConsume_NoRisk_NoChange()
    {
        var player = CreateTestPlayer();
        var consumable = new Consumable
        {
            Name = "Healing Potion",
            HealAmount = 25,
            AddictionRisk = 0
        };

        _addiction.OnConsume(player, consumable);

        Assert.Equal(0, player.AddictionLevel);
    }

    [Fact]
    public void OnConsume_CapsAtMax()
    {
        var player = CreateTestPlayer();
        player.AddictionLevel = 90;
        var consumable = new Consumable
        {
            Name = "Void Essence",
            SanityAmount = 100,
            AddictionRisk = 50
        };

        _addiction.OnConsume(player, consumable);

        Assert.Equal(100, player.AddictionLevel);
    }

    [Fact]
    public void Update_BelowThreshold_NoWithdrawal()
    {
        var player = CreateTestPlayer();
        player.AddictionLevel = 30;

        _addiction.Update(player);

        Assert.False(player.StatusEffects.HasEffect(StatusEffectType.Withdrawal));
    }

    [Fact]
    public void Update_AboveThreshold_AppliesWithdrawal()
    {
        var player = CreateTestPlayer();
        player.AddictionLevel = 50;

        _addiction.Update(player);

        Assert.True(player.StatusEffects.HasEffect(StatusEffectType.Withdrawal));
    }

    [Fact]
    public void Update_CravingMessageEvery10Turns()
    {
        var player = CreateTestPlayer();
        player.AddictionLevel = 60;

        for (int i = 0; i < 10; i++)
            _addiction.Update(player);

        Assert.Contains(_messages, m => m.Contains("crave"));
    }
}
