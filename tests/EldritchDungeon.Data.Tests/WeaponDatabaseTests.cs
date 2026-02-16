using EldritchDungeon.Core;
using EldritchDungeon.Data.Items;

namespace EldritchDungeon.Data.Tests;

public class WeaponDatabaseTests
{
    [Fact]
    public void GetAll_Returns34Weapons()
    {
        var all = WeaponDatabase.GetAll();
        Assert.Equal(34, all.Count);
    }

    [Fact]
    public void Medieval_Has10Weapons()
    {
        var weapons = WeaponDatabase.GetByCategory(WeaponCategory.Medieval).ToList();
        Assert.Equal(10, weapons.Count);
    }

    [Fact]
    public void EarlyModern_Has8Weapons()
    {
        var weapons = WeaponDatabase.GetByCategory(WeaponCategory.EarlyModern).ToList();
        Assert.Equal(8, weapons.Count);
    }

    [Fact]
    public void Dieselpunk_Has8Weapons()
    {
        var weapons = WeaponDatabase.GetByCategory(WeaponCategory.Dieselpunk).ToList();
        Assert.Equal(8, weapons.Count);
    }

    [Fact]
    public void Lovecraftian_Has8Weapons()
    {
        var weapons = WeaponDatabase.GetByCategory(WeaponCategory.Lovecraftian).ToList();
        Assert.Equal(8, weapons.Count);
    }

    [Fact]
    public void Dagger_HasCorrectStats()
    {
        var dagger = WeaponDatabase.Get("Dagger");
        Assert.Equal(4, dagger.Damage);
        Assert.Equal(18, dagger.CritRangeMin);
        Assert.Equal(2, dagger.CritMultiplier);
        Assert.Equal(1, dagger.Speed);
        Assert.Equal(WeaponCategory.Medieval, dagger.Category);
    }

    [Fact]
    public void HeartOfCthulhu_HasCorrectStats()
    {
        var heart = WeaponDatabase.Get("Heart of Cthulhu");
        Assert.Equal(100, heart.Damage);
        Assert.Equal(5, heart.CritMultiplier);
        Assert.Equal(WeaponCategory.Lovecraftian, heart.Category);
    }

    [Fact]
    public void AllWeapons_HavePositiveDamage_ExceptStarVisor()
    {
        var all = WeaponDatabase.GetAll();
        foreach (var kvp in all)
        {
            if (kvp.Key == "Star-Visor")
            {
                Assert.Equal(0, kvp.Value.Damage);
            }
            else
            {
                Assert.True(kvp.Value.Damage > 0, $"{kvp.Key} should have positive damage");
            }
        }
    }

    [Theory]
    [InlineData("Longsword", 12)]
    [InlineData("Flintlock Pistol", 20)]
    [InlineData("Clockwork Pistol", 28)]
    [InlineData("Star-Metal Blade", 35)]
    public void SpotCheck_WeaponDamage(string name, int expectedDamage)
    {
        var weapon = WeaponDatabase.Get(name);
        Assert.Equal(expectedDamage, weapon.Damage);
    }

    [Fact]
    public void FlintlockPistol_HasAmmo()
    {
        var pistol = WeaponDatabase.Get("Flintlock Pistol");
        Assert.Equal(6, pistol.MaxAmmo);
        Assert.Equal(6, pistol.CurrentAmmo);
    }

    [Fact]
    public void MeleeWeapons_HaveNoAmmo()
    {
        var dagger = WeaponDatabase.Get("Dagger");
        Assert.Equal(0, dagger.MaxAmmo);
    }
}
