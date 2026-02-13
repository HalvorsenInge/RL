using EldritchDungeon.Core;
using EldritchDungeon.Data.Classes;

namespace EldritchDungeon.Data.Tests;

public class ClassDatabaseTests
{
    [Fact]
    public void GetAll_Returns6Classes()
    {
        var classes = ClassDatabase.GetAll();
        Assert.Equal(6, classes.Count);
    }

    [Theory]
    [InlineData(ClassType.Warrior)]
    [InlineData(ClassType.Mage)]
    [InlineData(ClassType.Rogue)]
    [InlineData(ClassType.Cultist)]
    [InlineData(ClassType.Gunslinger)]
    [InlineData(ClassType.Investigator)]
    public void Get_AllClassesExist(ClassType classType)
    {
        var classDef = ClassDatabase.Get(classType);
        Assert.NotNull(classDef);
        Assert.Equal(classType, classDef.Type);
    }

    [Theory]
    [InlineData(ClassType.Warrior, 50)]
    [InlineData(ClassType.Mage, 30)]
    [InlineData(ClassType.Rogue, 75)]
    [InlineData(ClassType.Cultist, 25)]
    [InlineData(ClassType.Gunslinger, 40)]
    [InlineData(ClassType.Investigator, 60)]
    public void StartingGold_MatchesPlan(ClassType classType, int expectedGold)
    {
        var classDef = ClassDatabase.Get(classType);
        Assert.Equal(expectedGold, classDef.StartingGold);
    }

    [Fact]
    public void Warrior_HasCorrectEquipment()
    {
        var warrior = ClassDatabase.Get(ClassType.Warrior);
        Assert.Contains("Longsword", warrior.StartingWeapons);
        Assert.Equal("Chainmail", warrior.StartingArmor);
        Assert.Equal(3, warrior.StartingItems.Count(i => i == "Healing Potion"));
    }

    [Fact]
    public void Mage_HasCorrectEquipment()
    {
        var mage = ClassDatabase.Get(ClassType.Mage);
        Assert.Contains("Dagger", mage.StartingWeapons);
        Assert.Equal("Robe", mage.StartingArmor);
        Assert.Contains("Spellbook (Fireball)", mage.StartingItems);
        Assert.Equal(2, mage.StartingItems.Count(i => i == "Mana Potion"));
    }

    [Fact]
    public void Rogue_HasTwoWeapons()
    {
        var rogue = ClassDatabase.Get(ClassType.Rogue);
        Assert.Equal(2, rogue.StartingWeapons.Count);
        Assert.Contains("Shortbow", rogue.StartingWeapons);
        Assert.Contains("Dagger", rogue.StartingWeapons);
    }

    [Fact]
    public void AllClasses_HavePositiveBaseHp()
    {
        foreach (var classDef in ClassDatabase.GetAll().Values)
        {
            Assert.True(classDef.BaseHp > 0, $"{classDef.Name} should have positive base HP");
        }
    }

    [Fact]
    public void AllClasses_HaveNonNegativeBaseMana()
    {
        foreach (var classDef in ClassDatabase.GetAll().Values)
        {
            Assert.True(classDef.BaseMana >= 0, $"{classDef.Name} should have non-negative base mana");
        }
    }

    [Fact]
    public void AllClasses_HaveStartingArmor()
    {
        foreach (var classDef in ClassDatabase.GetAll().Values)
        {
            Assert.False(string.IsNullOrEmpty(classDef.StartingArmor),
                $"{classDef.Name} should have starting armor");
        }
    }
}
