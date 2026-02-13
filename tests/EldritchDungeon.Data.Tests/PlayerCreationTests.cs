using EldritchDungeon.Core;
using EldritchDungeon.Data.Races;
using EldritchDungeon.Data.Classes;
using EldritchDungeon.Entities;

namespace EldritchDungeon.Data.Tests;

public class PlayerCreationTests
{
    [Fact]
    public void CreateCharacter_HumanWarrior_HasValidStats()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Warrior);

        var player = Player.CreateCharacter(
            "Test Hero",
            race.Type,
            cls.Type,
            race.GetStatModifiers(),
            race.HpMultiplier,
            race.ManaMultiplier,
            race.SanityMultiplier,
            cls.BaseHp,
            cls.BaseMana,
            cls.StartingGold);

        Assert.Equal("Test Hero", player.Name);
        Assert.Equal(RaceType.Human, player.Race);
        Assert.Equal(ClassType.Warrior, player.Class);
        Assert.Equal(50, player.Gold);
        Assert.Equal('@', player.Glyph);
    }

    [Fact]
    public void CreateCharacter_HasPositiveHp()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Warrior);

        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        Assert.True(player.Health.MaxHp > 0);
        Assert.Equal(player.Health.MaxHp, player.Health.CurrentHp);
        Assert.False(player.Health.IsDead);
    }

    [Fact]
    public void CreateCharacter_HasValidSanity()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Warrior);

        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        Assert.True(player.Sanity.MaxSanity > 0);
        Assert.Equal(player.Sanity.MaxSanity, player.Sanity.CurrentSanity);
        Assert.Equal(SanityState.Stable, player.Sanity.State);
    }

    [Fact]
    public void CreateCharacter_StatsInValidRange()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Mage);

        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Mage,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        // Human has no modifiers, so stats should still be in rolled range
        Assert.InRange(player.Stats.Strength, 6, 18);
        Assert.InRange(player.Stats.Dexterity, 6, 18);
        Assert.InRange(player.Stats.Constitution, 6, 18);
        Assert.InRange(player.Stats.Intelligence, 6, 18);
        Assert.InRange(player.Stats.Wisdom, 6, 18);
        Assert.InRange(player.Stats.Charisma, 6, 18);
    }

    [Fact]
    public void CreateCharacter_ElfMage_HasRacialModifiersApplied()
    {
        Dice.SetSeed(42); // Deterministic for testing
        var race = RaceDatabase.Get(RaceType.Elf);
        var cls = ClassDatabase.Get(ClassType.Mage);

        var player = Player.CreateCharacter(
            "Elf Mage", RaceType.Elf, ClassType.Mage,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        // Elf has +1.2 mana multiplier, so mana should be higher than base
        Assert.True(player.Mana.MaxMana > 0);
        Assert.Equal(30, player.Gold);
    }

    [Fact]
    public void CreateCharacter_EquipmentSlotsInitialized()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Warrior);

        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Warrior,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        // All slots should be initialized (to null)
        foreach (EquipmentSlot slot in Enum.GetValues<EquipmentSlot>())
        {
            Assert.Null(player.Equipment.GetEquipped(slot));
        }
    }

    [Fact]
    public void CreateCharacter_AllRaceClassCombinations_Succeed()
    {
        foreach (RaceType raceType in Enum.GetValues<RaceType>())
        {
            var race = RaceDatabase.Get(raceType);
            foreach (ClassType classType in Enum.GetValues<ClassType>())
            {
                var cls = ClassDatabase.Get(classType);
                var player = Player.CreateCharacter(
                    $"{raceType} {classType}",
                    raceType, classType,
                    race.GetStatModifiers(),
                    race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
                    cls.BaseHp, cls.BaseMana, cls.StartingGold);

                Assert.NotNull(player);
                Assert.True(player.Health.MaxHp > 0,
                    $"{raceType} {classType} should have positive HP");
                Assert.True(player.Sanity.MaxSanity > 0,
                    $"{raceType} {classType} should have positive sanity");
            }
        }
    }

    [Fact]
    public void CreateCharacter_ReligionStartsEmpty()
    {
        var race = RaceDatabase.Get(RaceType.Human);
        var cls = ClassDatabase.Get(ClassType.Cultist);

        var player = Player.CreateCharacter(
            "Test", RaceType.Human, ClassType.Cultist,
            race.GetStatModifiers(),
            race.HpMultiplier, race.ManaMultiplier, race.SanityMultiplier,
            cls.BaseHp, cls.BaseMana, cls.StartingGold);

        Assert.Null(player.Religion.CurrentGod);
        Assert.Equal(0, player.Religion.Favor);
        Assert.Equal(0, player.Religion.Anger);
        Assert.Equal(0, player.Religion.PowerTier);
    }
}
