using EldritchDungeon.Core;
using EldritchDungeon.Data.Races;

namespace EldritchDungeon.Data.Tests;

public class RaceDatabaseTests
{
    [Fact]
    public void GetAll_Returns8Races()
    {
        var races = RaceDatabase.GetAll();
        Assert.Equal(8, races.Count);
    }

    [Theory]
    [InlineData(RaceType.Human)]
    [InlineData(RaceType.Elf)]
    [InlineData(RaceType.Dwarf)]
    [InlineData(RaceType.Halfling)]
    [InlineData(RaceType.Orc)]
    [InlineData(RaceType.DeepOneHybrid)]
    [InlineData(RaceType.HalfMad)]
    [InlineData(RaceType.SerpentFolk)]
    public void Get_AllRacesExist(RaceType raceType)
    {
        var race = RaceDatabase.Get(raceType);
        Assert.NotNull(race);
        Assert.Equal(raceType, race.Type);
    }

    [Fact]
    public void Human_HasNoModifiers()
    {
        var human = RaceDatabase.Get(RaceType.Human);
        Assert.Equal(0, human.StrMod);
        Assert.Equal(0, human.DexMod);
        Assert.Equal(0, human.ConMod);
        Assert.Equal(0, human.IntMod);
        Assert.Equal(0, human.WisMod);
        Assert.Equal(0, human.ChaMod);
        Assert.Equal(1.0, human.HpMultiplier);
        Assert.Equal(1.0, human.ManaMultiplier);
        Assert.Equal(1.0, human.SanityMultiplier);
    }

    [Fact]
    public void Elf_HasCorrectModifiers()
    {
        var elf = RaceDatabase.Get(RaceType.Elf);
        Assert.Equal(-1, elf.StrMod);
        Assert.Equal(2, elf.DexMod);
        Assert.Equal(-1, elf.ConMod);
        Assert.Equal(1, elf.IntMod);
        Assert.Equal(0, elf.WisMod);
        Assert.Equal(0, elf.ChaMod);
        Assert.Equal(0.9, elf.HpMultiplier);
        Assert.Equal(1.2, elf.ManaMultiplier);
        Assert.Equal(1.0, elf.SanityMultiplier);
    }

    [Fact]
    public void Dwarf_HasCorrectModifiers()
    {
        var dwarf = RaceDatabase.Get(RaceType.Dwarf);
        Assert.Equal(2, dwarf.StrMod);
        Assert.Equal(-1, dwarf.DexMod);
        Assert.Equal(2, dwarf.ConMod);
        Assert.Equal(0, dwarf.IntMod);
        Assert.Equal(0, dwarf.WisMod);
        Assert.Equal(-1, dwarf.ChaMod);
        Assert.Equal(1.3, dwarf.HpMultiplier);
        Assert.Equal(0.8, dwarf.ManaMultiplier);
        Assert.Equal(1.1, dwarf.SanityMultiplier);
    }

    [Fact]
    public void Orc_HasCorrectModifiers()
    {
        var orc = RaceDatabase.Get(RaceType.Orc);
        Assert.Equal(3, orc.StrMod);
        Assert.Equal(-1, orc.DexMod);
        Assert.Equal(2, orc.ConMod);
        Assert.Equal(-2, orc.IntMod);
        Assert.Equal(-1, orc.WisMod);
        Assert.Equal(-1, orc.ChaMod);
        Assert.Equal(1.4, orc.HpMultiplier);
        Assert.Equal(0.6, orc.ManaMultiplier);
        Assert.Equal(0.7, orc.SanityMultiplier);
    }

    [Fact]
    public void DeepOneHybrid_HasCorrectModifiers()
    {
        var doh = RaceDatabase.Get(RaceType.DeepOneHybrid);
        Assert.Equal(1, doh.StrMod);
        Assert.Equal(1, doh.DexMod);
        Assert.Equal(1, doh.ConMod);
        Assert.Equal(0, doh.IntMod);
        Assert.Equal(0, doh.WisMod);
        Assert.Equal(-1, doh.ChaMod);
        Assert.Equal(1.1, doh.HpMultiplier);
        Assert.Equal(1.0, doh.ManaMultiplier);
        Assert.Equal(0.5, doh.SanityMultiplier);
    }

    [Fact]
    public void HalfMad_HasLowestSanityMultiplier()
    {
        var halfMad = RaceDatabase.Get(RaceType.HalfMad);
        Assert.Equal(0.3, halfMad.SanityMultiplier);

        // Verify it's the lowest
        foreach (var race in RaceDatabase.GetAll().Values)
        {
            Assert.True(race.SanityMultiplier >= halfMad.SanityMultiplier);
        }
    }

    [Fact]
    public void GetStatModifiers_ReturnsCorrectArray()
    {
        var elf = RaceDatabase.Get(RaceType.Elf);
        var mods = elf.GetStatModifiers();

        Assert.Equal(6, mods.Length);
        Assert.Equal(-1, mods[0]); // STR
        Assert.Equal(2, mods[1]);  // DEX
        Assert.Equal(-1, mods[2]); // CON
        Assert.Equal(1, mods[3]);  // INT
        Assert.Equal(0, mods[4]);  // WIS
        Assert.Equal(0, mods[5]);  // CHA
    }
}
