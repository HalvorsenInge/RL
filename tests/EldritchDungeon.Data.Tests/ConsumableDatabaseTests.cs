using EldritchDungeon.Data.Items;

namespace EldritchDungeon.Data.Tests;

public class ConsumableDatabaseTests
{
    [Fact]
    public void GetAll_Returns9Consumables()
    {
        var all = ConsumableDatabase.GetAll();
        Assert.Equal(9, all.Count);
    }

    [Theory]
    [InlineData("Sanctified Water", 30, 0)]
    [InlineData("Elixir of Calm", 25, 0)]
    [InlineData("Mindcrust", 50, 30)]
    [InlineData("Deep One Whiskey", 40, 25)]
    [InlineData("Cultist Opium", 60, 40)]
    [InlineData("Void Essence", 100, 50)]
    public void SanityHealingItems_MatchPlan(string name, int expectedSanity, int expectedRisk)
    {
        var item = ConsumableDatabase.Get(name);
        Assert.Equal(expectedSanity, item.SanityAmount);
        Assert.Equal(expectedRisk, item.AddictionRisk);
    }

    [Fact]
    public void HealingPotion_HasCorrectAmount()
    {
        var potion = ConsumableDatabase.Get("Healing Potion");
        Assert.Equal(25, potion.HealAmount);
        Assert.Equal(0, potion.SanityAmount);
    }

    [Fact]
    public void ManaPotion_HasCorrectAmount()
    {
        var potion = ConsumableDatabase.Get("Mana Potion");
        Assert.Equal(25, potion.ManaAmount);
        Assert.Equal(0, potion.HealAmount);
    }

    [Fact]
    public void SanityPotion_HasCorrectAmount()
    {
        var potion = ConsumableDatabase.Get("Sanity Potion");
        Assert.Equal(30, potion.SanityAmount);
    }

    [Fact]
    public void HigherRiskItems_HealMoreSanity()
    {
        var mindcrust = ConsumableDatabase.Get("Mindcrust");
        var opium = ConsumableDatabase.Get("Cultist Opium");
        var voidEssence = ConsumableDatabase.Get("Void Essence");

        Assert.True(opium.SanityAmount > mindcrust.SanityAmount);
        Assert.True(voidEssence.SanityAmount > opium.SanityAmount);
        Assert.True(opium.AddictionRisk > mindcrust.AddictionRisk);
        Assert.True(voidEssence.AddictionRisk > opium.AddictionRisk);
    }
}
