using EldritchDungeon.Core;
using EldritchDungeon.Data.Gods;

namespace EldritchDungeon.Data.Tests;

public class GodDatabaseTests
{
    [Fact]
    public void GetAll_Returns6Gods()
    {
        var all = GodDatabase.GetAll();
        Assert.Equal(6, all.Count);
    }

    [Theory]
    [InlineData(GodType.Cthulhu, "Cthulhu", "Dreams")]
    [InlineData(GodType.Nyarlathotep, "Nyarlathotep", "Chaos")]
    [InlineData(GodType.Azathoth, "Azathoth", "Void")]
    [InlineData(GodType.YogSothoth, "Yog-Sothoth", "Knowledge")]
    [InlineData(GodType.Hastur, "Hastur", "Stars")]
    [InlineData(GodType.Dagon, "Dagon", "Deep")]
    public void Get_AllGodsExist(GodType type, string expectedName, string expectedDomain)
    {
        var god = GodDatabase.Get(type);
        Assert.Equal(expectedName, god.Name);
        Assert.Equal(expectedDomain, god.Domain);
    }

    [Fact]
    public void EachGod_Has4Powers()
    {
        var all = GodDatabase.GetAll();
        Assert.All(all.Values, god =>
            Assert.Equal(4, god.Powers.Count));
    }

    [Fact]
    public void PowerTiers_AreCorrect_25_50_75_100()
    {
        var all = GodDatabase.GetAll();
        foreach (var god in all.Values)
        {
            Assert.Equal(25, god.Powers[0].FavorRequired);
            Assert.Equal(50, god.Powers[1].FavorRequired);
            Assert.Equal(75, god.Powers[2].FavorRequired);
            Assert.Equal(100, god.Powers[3].FavorRequired);

            Assert.Equal(1, god.Powers[0].Tier);
            Assert.Equal(2, god.Powers[1].Tier);
            Assert.Equal(3, god.Powers[2].Tier);
            Assert.Equal(4, god.Powers[3].Tier);
        }
    }

    [Fact]
    public void Cthulhu_HasCorrectPowers()
    {
        var cthulhu = GodDatabase.Get(GodType.Cthulhu);
        Assert.Equal("Dreams", cthulhu.Powers[0].Name);
        Assert.Equal("Tentacle Slam", cthulhu.Powers[1].Name);
        Assert.Equal("Cult Summon", cthulhu.Powers[2].Name);
        Assert.Equal("R'lyeh Rising", cthulhu.Powers[3].Name);
    }

    [Fact]
    public void AllGods_HaveNonEmptyDomain()
    {
        var all = GodDatabase.GetAll();
        Assert.All(all.Values, god =>
            Assert.False(string.IsNullOrEmpty(god.Domain)));
    }

    [Fact]
    public void AllPowers_HaveNonEmptyDescription()
    {
        var all = GodDatabase.GetAll();
        foreach (var god in all.Values)
        {
            Assert.All(god.Powers, power =>
                Assert.False(string.IsNullOrEmpty(power.Description)));
        }
    }
}
