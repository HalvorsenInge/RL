using EldritchDungeon.Data.Monsters;

namespace EldritchDungeon.Data.Tests;

public class MonsterDatabaseTests
{
    [Fact]
    public void GetAll_Returns40Monsters()
    {
        var all = MonsterDatabase.GetAll();
        Assert.Equal(40, all.Count);
    }

    [Fact]
    public void TierGroup1_2_Has8Monsters()
    {
        var monsters = MonsterDatabase.GetByTierGroup(1, 2).ToList();
        Assert.Equal(8, monsters.Count);
    }

    [Fact]
    public void TierGroup3_4_Has8Monsters()
    {
        var monsters = MonsterDatabase.GetByTierGroup(3, 4).ToList();
        Assert.Equal(8, monsters.Count);
    }

    [Fact]
    public void TierGroup5_6_Has8Monsters()
    {
        var monsters = MonsterDatabase.GetByTierGroup(5, 6).ToList();
        Assert.Equal(8, monsters.Count);
    }

    [Fact]
    public void TierGroup7_8_Has8Monsters()
    {
        var monsters = MonsterDatabase.GetByTierGroup(7, 8).ToList();
        Assert.Equal(8, monsters.Count);
    }

    [Fact]
    public void TierGroup9_10_Has8Monsters()
    {
        var monsters = MonsterDatabase.GetByTierGroup(9, 10).ToList();
        Assert.Equal(8, monsters.Count);
    }

    [Fact]
    public void Rat_Has5HP()
    {
        var rat = MonsterDatabase.Get("Rat");
        Assert.Equal(5, rat.HP);
        Assert.Equal(2, rat.Damage);
        Assert.Equal(0, rat.SanityDamage);
        Assert.Equal(10, rat.XpValue);
    }

    [Fact]
    public void Cthulhu_Has666HP()
    {
        var cthulhu = MonsterDatabase.Get("Cthulhu");
        Assert.Equal(666, cthulhu.HP);
        Assert.Equal(150, cthulhu.Damage);
        Assert.Equal(200, cthulhu.SanityDamage);
        Assert.Equal(10000, cthulhu.XpValue);
        Assert.Equal(10, cthulhu.Tier);
    }

    [Fact]
    public void GetByTier_ReturnsCorrectSubset()
    {
        var tier1 = MonsterDatabase.GetByTier(1).ToList();
        Assert.All(tier1, m => Assert.Equal(1, m.Tier));
        Assert.Contains(tier1, m => m.Name == "Rat");
        Assert.Contains(tier1, m => m.Name == "Goblin");
    }

    [Fact]
    public void AllMonsters_HavePositiveHP()
    {
        var all = MonsterDatabase.GetAll();
        Assert.All(all.Values, m => Assert.True(m.HP > 0, $"{m.Name} should have positive HP"));
    }

    [Fact]
    public void AllMonsters_HaveNonNegativeXP()
    {
        var all = MonsterDatabase.GetAll();
        Assert.All(all.Values, m => Assert.True(m.XpValue > 0, $"{m.Name} should have positive XP"));
    }

    [Fact]
    public void AllMonsters_HaveAtLeastOneAbility()
    {
        var all = MonsterDatabase.GetAll();
        Assert.All(all.Values, m => Assert.True(m.Abilities.Count > 0, $"{m.Name} should have abilities"));
    }

    [Theory]
    [InlineData("Deep One", 5, 50, 18, 15, 200)]
    [InlineData("Shoggeth", 7, 100, 35, 35, 450)]
    [InlineData("Great Old One", 10, 500, 100, 100, 5000)]
    public void SpotCheck_MonsterStats(string name, int tier, int hp, int damage, int sanity, int xp)
    {
        var monster = MonsterDatabase.Get(name);
        Assert.Equal(tier, monster.Tier);
        Assert.Equal(hp, monster.HP);
        Assert.Equal(damage, monster.Damage);
        Assert.Equal(sanity, monster.SanityDamage);
        Assert.Equal(xp, monster.XpValue);
    }
}
