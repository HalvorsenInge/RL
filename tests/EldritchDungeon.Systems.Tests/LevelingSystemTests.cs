using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Systems;

namespace EldritchDungeon.Systems.Tests;

public class LevelingSystemTests
{
    private readonly List<string> _messages = new();
    private readonly LevelingSystem _leveling;

    public LevelingSystemTests()
    {
        _leveling = new LevelingSystem(msg => _messages.Add(msg));
    }

    private static Player CreateTestPlayer()
    {
        var player = new Player { Name = "Tester" };
        player.InitializeComponents();
        player.Stats.Strength = 14;
        player.Stats.Dexterity = 12;
        player.Stats.Constitution = 14; // +2 modifier
        player.Stats.Intelligence = 12; // +1 modifier
        player.Stats.Wisdom = 10;
        player.Stats.Charisma = 10;
        player.Health.MaxHp = 40;
        player.Health.CurrentHp = 30;
        player.Mana.MaxMana = 10;
        player.Mana.CurrentMana = 5;
        player.Stats.Level = 1;
        player.Stats.Experience = 0;
        player.Stats.ExperienceToNextLevel = 100;
        return player;
    }

    [Fact]
    public void CheckLevelUp_NoLevelUpBelowThreshold()
    {
        var player = CreateTestPlayer();
        player.Stats.Experience = 50;

        _leveling.CheckLevelUp(player);

        Assert.Equal(1, player.Stats.Level);
    }

    [Fact]
    public void CheckLevelUp_LevelsUpAtThreshold()
    {
        var player = CreateTestPlayer();
        player.Stats.Experience = 100;

        _leveling.CheckLevelUp(player);

        Assert.Equal(2, player.Stats.Level);
        Assert.Equal(0, player.Stats.Experience);
    }

    [Fact]
    public void CheckLevelUp_IncreasesHpByConMod()
    {
        var player = CreateTestPlayer();
        int hpBefore = player.Health.MaxHp;
        player.Stats.Experience = 100;

        _leveling.CheckLevelUp(player);

        int conMod = Dice.GetModifier(player.Stats.Constitution); // +2
        Assert.Equal(hpBefore + Math.Max(1, conMod), player.Health.MaxHp);
        Assert.Equal(player.Health.MaxHp, player.Health.CurrentHp); // Fully healed
    }

    [Fact]
    public void CheckLevelUp_IncreasesManaByIntMod()
    {
        var player = CreateTestPlayer();
        int manaBefore = player.Mana.MaxMana;
        player.Stats.Experience = 100;

        _leveling.CheckLevelUp(player);

        int intMod = Dice.GetModifier(player.Stats.Intelligence); // +1
        Assert.Equal(manaBefore + Math.Max(0, intMod), player.Mana.MaxMana);
        Assert.Equal(player.Mana.MaxMana, player.Mana.CurrentMana); // Fully restored
    }

    [Fact]
    public void CheckLevelUp_XpThresholdDoubles()
    {
        var player = CreateTestPlayer();
        player.Stats.Experience = 100;

        _leveling.CheckLevelUp(player);

        Assert.Equal(200, player.Stats.ExperienceToNextLevel); // 100 * 2^1
    }

    [Fact]
    public void CheckLevelUp_StatBoostAtLevel3()
    {
        Dice.SetSeed(42);
        var player = CreateTestPlayer();

        // Level 1 -> 2 -> 3 (need 100 + 200 = 300 XP)
        player.Stats.Experience = 300;

        int totalStatsBefore = player.Stats.Strength + player.Stats.Dexterity +
            player.Stats.Constitution + player.Stats.Intelligence +
            player.Stats.Wisdom + player.Stats.Charisma;

        _leveling.CheckLevelUp(player);

        int totalStatsAfter = player.Stats.Strength + player.Stats.Dexterity +
            player.Stats.Constitution + player.Stats.Intelligence +
            player.Stats.Wisdom + player.Stats.Charisma;

        Assert.Equal(3, player.Stats.Level);
        Assert.Equal(totalStatsBefore + 1, totalStatsAfter);
    }

    [Fact]
    public void CheckLevelUp_MultipleLevelsAtOnce()
    {
        var player = CreateTestPlayer();
        player.Stats.Experience = 300; // 100 + 200 = level 3

        _leveling.CheckLevelUp(player);

        Assert.Equal(3, player.Stats.Level);
    }
}
